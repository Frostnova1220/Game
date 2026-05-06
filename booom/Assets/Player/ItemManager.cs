using UnityEngine;
using System.Collections.Generic;

public class ItemManager : MonoBehaviour
{
    public LayerMask whatIsEnemy;
    [System.Serializable]
    public class ItemSlot
    {
        public PickupItem.ItemType itemType;
        public int usesLeft;
    }

    public List<ItemSlot> items = new List<ItemSlot>();
    private PickupItem.ItemType? equippedItem = null;

    [Header("榴弹")]
    public GameObject grenadeBulletPrefab;
    public float explosionRadius = 3f;
    public float explosionDamage = 30f;

    [Header("虫洞")]
    public GameObject wormholePrefab;
    public float wormholeLifetime = 20f;
    private GameObject firstWormhole = null;
    private bool wormholeUsed = false;

    [Header("人偶")]
    public GameObject decoyPrefab;
    public float decoyDuration = 10f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            EquipItem(PickupItem.ItemType.Grenade);
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            EquipItem(PickupItem.ItemType.Wormhole);
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            EquipItem(PickupItem.ItemType.Decoy);
    }

    public void AddItem(PickupItem.ItemType type)
    {
        ItemSlot existing = items.Find(s => s.itemType == type);
        if (existing != null)
        {
            existing.usesLeft += 3;
            if (existing.usesLeft > 3) existing.usesLeft = 3;
        }
        else
        {
            items.Add(new ItemSlot { itemType = type, usesLeft = 3 });
        }
        Debug.Log($"获得道具: {type}，剩余次数: {(existing != null ? existing.usesLeft : 3)}");
    }

    void EquipItem(PickupItem.ItemType type)
    {
        ItemSlot slot = items.Find(s => s.itemType == type);
        if (slot == null || slot.usesLeft <= 0)
        {
            Debug.Log($"没有道具或次数用完: {type}");
            equippedItem = null;
            return;
        }
        equippedItem = type;
        Debug.Log($"装备道具: {type}，剩余次数: {slot.usesLeft}");
    }

    // 玩家攻击时调用
    public bool TryUseEquippedItem(Vector3 playerPos, Vector3 aimDir, GameObject normalBulletPrefab, Transform firePoint, LayerMask enemyLayer, out GameObject spawnedBullet)
    {
        spawnedBullet = null;
        if (equippedItem == null) return false;

        ItemSlot slot = items.Find(s => s.itemType == equippedItem.Value);
        if (slot == null || slot.usesLeft <= 0)
        {
            equippedItem = null;
            return false;
        }

        switch (equippedItem.Value)
        {
            case PickupItem.ItemType.Grenade:
                spawnedBullet = UseGrenade(playerPos, aimDir, normalBulletPrefab, firePoint);
                break;
            case PickupItem.ItemType.Wormhole:
                UseWormhole(playerPos);
                break;
            case PickupItem.ItemType.Decoy:
                UseDecoy(playerPos);
                break;
        }

        // 只有榴弹和人偶立刻扣次数；虫洞在第二个放下时扣
        if (equippedItem.Value != PickupItem.ItemType.Wormhole)
        {
            slot.usesLeft--;
            Debug.Log($"使用道具: {equippedItem.Value}，剩余次数: {slot.usesLeft}");
            if (slot.usesLeft <= 0)
            {
                items.Remove(slot);
                equippedItem = null;
            }
        }

        return true;
    }

    // ===== 榴弹 =====
    GameObject UseGrenade(Vector3 pos, Vector3 dir, GameObject normalBulletPrefab, Transform firePoint)
    {
        Vector3 origin = firePoint != null ? firePoint.position : pos;

        // 用普通追踪子弹的预制体生成榴弹
        GameObject bullet = Instantiate(normalBulletPrefab, origin, Quaternion.LookRotation(dir));

        // 找到最近的敌人传给追踪目标
        Collider[] hits = Physics.OverlapSphere(origin, 15f, whatIsEnemy);
        Transform closest = null;
        float closestDist = 15f;
        for (int i = 0; i < hits.Length; i++)
        {
            float d = Vector3.Distance(origin, hits[i].transform.position);
            if (d < closestDist)
            {
                closestDist = d;
                closest = hits[i].transform;
            }
        }

        // 挂上榴弹爆炸组件
        HomingGrenadeBullet gb = bullet.AddComponent<HomingGrenadeBullet>();
        gb.speed = 10f;
        gb.turnRate = 8f;
        gb.whatIsEnemy = whatIsEnemy;
        gb.explosionRadius = explosionRadius;
        gb.explosionDamage = explosionDamage;
        gb.directDamage = 10f;
        gb.SetTarget(closest);

        return bullet;
    }  

    // ===== 虫洞 =====
    void UseWormhole(Vector3 playerPos)
    {
        if (firstWormhole == null)
        {
            // 第一个虫洞：放在玩家身边
            firstWormhole = Instantiate(wormholePrefab, playerPos, Quaternion.identity);
            Debug.Log("第一个虫洞已放置，再按一次攻击放置第二个并激活传送");
            // 不扣次数
        }
        else
        {
            // 第二个虫洞：放在玩家当前位置
            GameObject secondWormhole = Instantiate(wormholePrefab, playerPos, Quaternion.identity);

            // 双向传送
            WormholeExit exit1 = firstWormhole.GetComponent<WormholeExit>();
            if (exit1 == null)
                exit1 = firstWormhole.AddComponent<WormholeExit>();
            exit1.teleportTarget = secondWormhole.transform.position;

            WormholeExit exit2 = secondWormhole.GetComponent<WormholeExit>();
            if (exit2 == null)
                exit2 = secondWormhole.AddComponent<WormholeExit>();
            exit2.teleportTarget = firstWormhole.transform.position;

            // 两个虫洞 20 秒后消失
            Destroy(firstWormhole, wormholeLifetime);
            Destroy(secondWormhole, wormholeLifetime);

            // 扣次数
            ItemSlot slot = items.Find(s => s.itemType == PickupItem.ItemType.Wormhole);
            if (slot != null)
            {
                slot.usesLeft--;
                Debug.Log($"虫洞对已创建，20秒后消失。剩余次数: {slot.usesLeft}");
                if (slot.usesLeft <= 0)
                {
                    items.Remove(slot);
                    equippedItem = null;
                }
            }

            firstWormhole = null;
        }
    }

    // ===== 人偶 =====
    void UseDecoy(Vector3 playerPos)
    {
        GameObject decoy = Instantiate(decoyPrefab, playerPos, Quaternion.identity);
        Destroy(decoy, decoyDuration);
        Debug.Log("人偶已放置，10秒后消失");
    }
}
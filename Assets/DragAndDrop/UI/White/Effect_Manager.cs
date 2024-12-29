using System.Collections.Generic;
using UnityEngine;

public class Effect_Manager : MonoBehaviour
{
    public static Effect_Manager Instance { get; private set; } // Singleton Instance

    [System.Serializable]
    public class Pool
    {
        public string tag; // ชื่อแท็กสำหรับเรียกใช้ Pool
        public GameObject prefab; // Prefab ที่ต้องการสร้าง
        public int size; // จำนวนเริ่มต้นของ Object ใน Pool
    }

    public List<Pool> pools; // ลิสต์ของ Pool ทั้งหมด
    private Dictionary<string, Queue<GameObject>> poolDictionary; // จัดเก็บ Object ตามแท็ก

    private void Awake()
    {
        // ตรวจสอบ Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // สร้าง Dictionary สำหรับจัดเก็บ Object Pools
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            // สร้าง Object ตามจำนวนที่กำหนด
            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.tag, objectPool);
        }
    }

    // ฟังก์ชันสำหรับดึง Object จาก Pool
    public GameObject GetFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("ไม่มี " + tag);
            return null;
        }

        foreach (var obj in poolDictionary[tag])
        {
            if (!obj.activeSelf)
            {
                obj.SetActive(true);
                obj.transform.position = position;
                obj.transform.rotation = rotation;

                HideEffect hideEffect = obj.GetComponent<HideEffect>();
                if (hideEffect != null)
                {
                    hideEffect.Show();
                }

                return obj;
            }
        }

        return null;
    }

}

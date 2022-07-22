using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooling : MonoBehaviour
{
    private GameObject example;
    private Transform storage;

    private Queue<GameObject> poolOfObjects;

    public ObjectPooling(int Index, GameObject Example, Transform Storage)
    {
        example = Example;
        storage = Storage;

        poolOfObjects = new Queue<GameObject>();


        for (int i = 0; i < Index; i++)
        {
            GameObject _object = Instantiate(Example, Storage);
            _object.SetActive(false);
            poolOfObjects.Enqueue(_object);
        }
    }

    public ObjectPooling(int Index, Transform Storage)
    {

        storage = Storage;

        poolOfObjects = new Queue<GameObject>();


        for (int i = 0; i < Index; i++)
        {
            int panel_number = Random.Range(1, 6);
            GameObject _object = Instantiate(GetPanelByNumber(panel_number), Storage);
            _object.SetActive(false);
            //_object.GetComponent<Panel>().CurrentType = (PanelTypes)panel_number;            
            poolOfObjects.Enqueue(_object);
        }
    }



    public ObjectPooling(int Index, GameObject Example)
    {
        example = Example;
        storage = null;

        poolOfObjects = new Queue<GameObject>();


        for (int i = 0; i < Index; i++)
        {
            GameObject _object = Instantiate(Example);
            _object.SetActive(false);
            poolOfObjects.Enqueue(_object);
        }
    }

    public GameObject GetObject()
    {
        if (poolOfObjects.Count > 0) return poolOfObjects.Dequeue();

        print("instantiated new object of type: queue is full");
        GameObject _object = null;
        if (storage == null)
        {
            Instantiate(example);
        }
        else
        {
            Instantiate(example, storage);
        }

        _object.SetActive(false);

        return _object;
    }

    public GameObject GetObject(bool isActiveBeforeTaken)
    {
        if (poolOfObjects.Count > 0)
        {
            GameObject result = poolOfObjects.Dequeue();
            result.SetActive(isActiveBeforeTaken);
            return result;
        }


        print("instantiated new object of type: queue is full");
        GameObject _object = null;
        if (storage == null)
        {
            Instantiate(example);
        }
        else
        {
            Instantiate(example, storage);
        }

        _object.SetActive(isActiveBeforeTaken);

        return _object;
    }

    public void ReturnObject(GameObject _object)
    {
        _object.SetActive(false);
        poolOfObjects.Enqueue(_object);
    }

    public static GameObject GetPanelByNumber(int number)
    {
        GameObject result = default;

        switch (number)
        {
            case 1:
                result = Resources.Load<GameObject>("panel1");
                break;

            case 2:
                result = Resources.Load<GameObject>("panel2");
                break;

            case 3:
                result = Resources.Load<GameObject>("panel3");
                break;

            case 4:
                result = Resources.Load<GameObject>("panel4");
                break;

            case 5:
                result = Resources.Load<GameObject>("panel5");
                break;
        }

        return result;
    }

}

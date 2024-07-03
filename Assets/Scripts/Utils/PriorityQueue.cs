using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class PriorityQueue<T> where T : UnityEngine.Object
{
    private SortedDictionary<float, Queue<T>> queue = new SortedDictionary<float, Queue<T>>();
    private float distanceEpsilon = 0.00001f;  // �Ÿ� ���ϼ��� ���� ������ ��

    public void Enqueue(float distance, T target)
    {
        

        // �Ÿ��� ���� ��츦 ó���ϱ� ���� �Ÿ��� ������ ���� ���� �߰�
        while (queue.ContainsKey(distance))
        {
            distance += UnityEngine.Random.Range(0f, distanceEpsilon);
        }

        if (!queue.ContainsKey(distance))
        {
            queue[distance] = new Queue<T>();
        }

        queue[distance].Enqueue(target);
    }

    public T Dequeue()
    {
        if (queue.Count == 0)
        {
            Debug.Log("�ش� ���� ����");
            return null;
        }

        var firstPair = queue.GetEnumerator();
        firstPair.MoveNext();
        var firstQueue = firstPair.Current.Value;
        T target = firstQueue.Dequeue();

        if (firstQueue.Count == 0)
        {
            queue.Remove(firstPair.Current.Key);
        }

        return target;
    }

    public T Peek()
    {
        if (queue.Count == 0)
        {
            Debug.Log("ť�� ��� ����");
            return null;
        }

        var firstPair = queue.GetEnumerator();

        firstPair.MoveNext();
        return firstPair.Current.Value.Peek(); 
    }

    public int Count()
    {
        int count = 0;
        foreach (var pair in queue)
        {
            count += pair.Value.Count;
        }
        return count;
    }

    public void Clear()
    {
        queue.Clear();
    }
}

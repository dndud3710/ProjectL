using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class PriorityQueue<T> where T : UnityEngine.Object
{
    private SortedDictionary<float, Queue<T>> queue = new SortedDictionary<float, Queue<T>>();
    private float distanceEpsilon = 0.00001f;  // 거리 유일성을 위한 무작위 값

    public void Enqueue(float distance, T target)
    {
        

        // 거리가 같을 경우를 처리하기 위해 거리에 무작위 작은 값을 추가
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
            Debug.Log("해당 사항 없음");
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
            Debug.Log("큐가 비어 있음");
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

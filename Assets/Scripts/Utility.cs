using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utility
{
   public static T[] ShuffleArray<T>(T[] array, int seed) //T타입 배열 ㄹ
    {
        System.Random prng = new System.Random(seed);

        //배열 모든 원소 거쳐 루프

        for(int i = 0; i<array.Length -1; i++) //-1이유는 마지막 루프는 무시되기에
        {
            int randomIndex = prng.Next(i, array.Length); // i 와 end  = array.Length로 끝을 알 수 있음
            T tempItem = array[randomIndex];
            array[randomIndex] = array[i];
            array[i] = tempItem;

        }

        return array; 
    }
}

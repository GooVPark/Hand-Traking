using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumberGameManager : MonoBehaviour
{
    public List<Number> numbers;
    public List<PosePlace> pose;

    /*
    public void RandomPlaceNumber()
    {
        for (int i = 0; i < numbers.Count; i++)
        {
            numbers[i].transform.position = pose
        }
    }
   */



    private void Start()
    {
        //Debug.Log(RandomPlaceNumber());
    }
}

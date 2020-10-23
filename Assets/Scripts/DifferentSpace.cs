using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifferentSpace : MonoBehaviour
{
    [SerializeField] Material mat;
    Color spaceColor = new Color(0, 0, 1, 5);




    void Start()
    {
        mat.color = spaceColor;
    }

    void Update()
    {
        
    }

    public void Growth ()
    {
        Debug.Log("異空間が成長");
    }
}

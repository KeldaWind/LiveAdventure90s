using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Following_Plateform : MonoBehaviour
{
    [Header("Plateform Movement")]
    [SerializeField] private Transform plateformPos;

    //public cameraRef
    [SerializeField] private Vector2 nextPos;
    private float plateformSpeed;
    private float yMax;
    private float yMin;

    [SerializeField] private bool inCameraRange;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ComparePlaterformAndCameraPositions()
    {
        //if()
        //{
        //    nextPos = 
        //}
    }

    void MoveToNextPos()
    {
        plateformPos.position = new Vector3(plateformPos.position.x, nextPos.y, plateformPos.position.z);
    }

    bool IsThereObstacles()
    {
        //RaycastHit2D upResult = Physics2D.BoxCast(plateformPos.position, )

        return false;
    }
}

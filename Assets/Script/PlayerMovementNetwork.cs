using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerMovementNetwork : NetworkBehaviour
{
    // Start is called before the first frame update
    float moveSpeed = 3f;

    private readonly NetworkVariable<Vector3> _newPos = new(writePerm: NetworkVariableWritePermission.Owner);
    private readonly NetworkVariable<Quaternion> _newRot = new(writePerm: NetworkVariableWritePermission.Owner);

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (IsOwner){       // update to server position
            _newPos.Value = transform.position;
            _newRot.Value = transform.rotation;

        }
        else{
            transform.position =  _newPos.Value;
            transform.rotation = _newRot.Value;
        }


        if (!IsOwner) return;
                
        Vector3 moveDir = new Vector3(0,0,0);
        if (Input.GetKey(KeyCode.W)) moveDir.z = +1f;
        if (Input.GetKey(KeyCode.S)) moveDir.z = -1f;
        if (Input.GetKey(KeyCode.A)) moveDir.x = -1f;
        if (Input.GetKey(KeyCode.D)) moveDir.x = +1f;

        transform.position += moveDir * moveSpeed *Time.deltaTime;


    }
}

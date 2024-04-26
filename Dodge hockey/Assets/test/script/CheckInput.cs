using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckInput : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //  Prefab�̃��[�h
        GameObject prefab = (GameObject)Resources.Load("Cube");
        Instantiate(prefab, new Vector3(0, 0, 0), Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        
        //  �}�E�X�{�^���̃A�b�v�𔻒�
        if(Input.GetMouseButtonUp(0))
        {
            //  ���C�L���X�g�̐ݒ�
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            //  �����蔻��
            if(Physics.Raycast(ray,out hit, 100))
            {
                if (null != hit.collider.gameObject)
                {
                    print("hit!  " + hit.collider.gameObject.name);
                }
            }
        }
    }
}

using UnityEngine;
using System.Collections;

public class Movement : MonoBehaviour {
    void Update() {
        Animator anim;
        anim = gameObject.GetComponent<Animator>();
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Walk")) {
            transform.Translate(Vector3.forward * Time.deltaTime);
            transform.Translate(Vector3.up * Time.deltaTime, Space.World);
        }
    }
}

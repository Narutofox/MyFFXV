using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonMove : MonoBehaviour
{
    // Start is called before the first frame update
    private Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        float speed = 0;
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Walk")) speed = 1;
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Run")) speed = 2;

        var position = transform.position;
        position += transform.forward * speed * Time.deltaTime;
        transform.position = position;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {

    private Animator anim;
    private SpriteRenderer spriteRenderer;

	public event SimpleDelegate onHit;
    public bool isHitted;

    void Start(){
        anim = transform.GetChild(0).GetComponent<Animator>();
        spriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
    }

    public void GetHitted(out bool hittedMe){
        if (isHitted == false){
            StartCoroutine(GetHittedInfo());
			if (onHit != null) {
				onHit ();
			}
            isHitted = true;
            hittedMe = true;
        }
        else{
            hittedMe = false;
        }
    }

    IEnumerator GetHittedInfo(){
        spriteRenderer.color = new Color(1, 0, 0);
        anim.speed = 0f;
        yield return new WaitForSeconds(0.3f);
        spriteRenderer.color = new Color(1, 1, 1);
        anim.speed = 1f;
        isHitted = false;
    }
}

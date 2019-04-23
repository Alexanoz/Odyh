﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

public class Player : Character
{
    private SFXManager sfx;

    public GameObject projectile;

    [SerializeField]
    private GameObject startpoint;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        startpoint = GameObject.Find("Startpoint");
        sfx = FindObjectOfType<SFXManager>();
    }

    // Update is called once per frame
    protected override void Update()
    {
         if (stopmove)
             return;
         
         Getinput();
         base.Update();
    }
    private void Getinput()
    {
        direction = Vector2.zero;
        if (Input.GetKey(KeyCode.Z))
        {
            direction += Vector2.up;
        }
        if (Input.GetKey(KeyCode.S))
        {
            direction += Vector2.down;
        }
        if (Input.GetKey(KeyCode.Q))
        {
            direction += Vector2.left;
        }
        if (Input.GetKey(KeyCode.D))
        {
            direction += Vector2.right;
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (!IsAttacking)
            // Allow to put a delay in the method
            StartCoroutine(Attack());
        }

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            if (!IsAttackingrange)
            {
                Vector3 way = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
                
                GameObject Arc = GameObject.FindGameObjectWithTag("Arc");

                
                Range projectile = Instantiate(this.projectile, transform.position, transform.rotation)
                    .GetComponent<Range>();
            
                myAnimator.SetFloat("x", way.x);
                myAnimator.SetFloat("y", way.y);
                
                projectile.Setup(way);
            
                projectile.Isattacking = true;

                StartCoroutine(Attackrange());
            }
        }
    }

    
    private IEnumerator Attack()
    {
        sfx.player_is_attacking.Play();
        IsAttacking = true;
        myAnimator.SetBool("attack", IsAttacking);

        // delay between each attack
        yield return new WaitForSeconds(0.4f);

        
        
        StopAttack();
    }
    
    private IEnumerator Attackrange()
    {
        sfx.player_is_attacking.Play();
        IsAttackingrange= true;
        myAnimator.SetBool("attackrange", IsAttackingrange);

        // delay between each attack
        yield return new WaitForSeconds(0.3f);

        
        
        StopAttackrange();
    }


}

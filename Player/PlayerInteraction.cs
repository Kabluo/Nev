using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerInteraction : MonoBehaviour
{
    private GameObject[] enemyScan;
    private GameObject[] enemyByProxy;
    public bool isExecuting;
    [SerializeField] float interactionRange = 3f;
    
    // Update is called once per frame
    void Update()
    {
        if(!PlayerController.instance.isAlive || PlayerController.instance.isInScene) { return; }
        
        if(isExecuting)
        {
            isExecuting = false;
            PlayerController.instance.myAnimator.SetBool("isExecuting", isExecuting);
            return;
        }

        ExecuteEnemy();
    }

    //Physics2D.OverlapCircle(transform.position, interactionRange, LayerMask.GetMask("Enemy")) - might use at some point
    //also refactor later, move into PlayerController
    void ExecuteEnemy()
    {
        if(Input.GetKeyDown(KeyCode.E) && !PlayerController.instance.myAnimator.GetCurrentAnimatorStateInfo(0).IsTag("PreventMovement"))
        {
            enemyScan = GameObject.FindGameObjectsWithTag("Enemy");
            if(enemyScan.Length == 0) { return; }

            //if enemy within range,  execute
            if(enemyScan.Length == 1)
            {
                //if(enemyScan[0].GetComponent<EnemyHealthController>().isBroken && (Mathf.Abs(enemyScan[0].transform.position.x - transform.position.x) + Mathf.Abs(enemyScan[0].transform.position.y - transform.position.y)) <= interactionRange)
                if(enemyScan[0].GetComponent<EnemyHealthController>().isBroken && Vector3.Distance(gameObject.transform.position, enemyScan[0].transform.position) < interactionRange)
                {
                    isExecuting = true;
                    PlayerController.instance.myAnimator.SetBool("isExecuting", isExecuting);
                    PlayerController.instance.myAnimator.SetInteger("executionId", enemyScan[0].GetComponent<EnemyHealthController>().enemyId);
                    enemyScan[0].GetComponent<EnemyHealthController>().Execute();

                    return;
                }
            }

            else
            {
                enemyByProxy = enemyScan.OrderBy(t=>(t.transform.position - transform.position).sqrMagnitude).ToArray(); //sort enemies by distance, get closest 3 !!!REMOVED .take(3) !!!
                for(int i = 0; i < enemyByProxy.Length; i++)
                {
                    //Debug.Log(Vector3.Distance(transform.position, enemyByProxy[i].transform.position));
                    if(enemyByProxy[i].GetComponent<EnemyHealthController>().isBroken && Vector3.Distance(gameObject.transform.position, enemyByProxy[i].transform.position) < interactionRange)  //manual range calculation available  at line 39
                    {
                        isExecuting = true;
                        PlayerController.instance.myAnimator.SetBool("isExecuting", isExecuting);
                        PlayerController.instance.myAnimator.SetInteger("executionId", enemyByProxy[i].GetComponent<EnemyHealthController>().enemyId);
                        enemyByProxy[i].GetComponent<EnemyHealthController>().Execute();

                        return;
                    }
                }
            }
        }
    }
}

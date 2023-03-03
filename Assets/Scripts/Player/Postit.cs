using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Postit : MonoBehaviour
{
    [HideInInspector] public Collider2D col;
    public ContactFilter2D lineContacts, nonLineContacts;
    WaitForFixedUpdate waiter;
    private void Start()
    {
        col = GetComponent<Collider2D>();
       // waiter = new WaitForFixedUpdate();
    }

    private void FixedUpdate()
    {
       // AvoidCollisionWithLines();
     
    }


    private void AvoidCollisionWithLine()
    {
        Collider2D[] lineColls = new Collider2D[100];
        Collider2D[] nonLineColls = new Collider2D[100];
        if (Physics2D.OverlapCollider(col, lineContacts, lineColls) > 0 && Physics2D.OverlapCollider(col, nonLineContacts, nonLineColls) > 0)
        {

            List<EdgeCollider2D> edgeCols = new List<EdgeCollider2D>();
            foreach (var coll in lineColls)
            {
                edgeCols.Add(coll as EdgeCollider2D);
            }



            List<Collider2D> nonEdgeCols = new List<Collider2D>();
            foreach (var coll in lineColls)
            {
                nonEdgeCols.Add(coll as EdgeCollider2D);
            }

            for (int i = 0; i < edgeCols.Count; i++)
            {

                for (int j = 0; j < nonEdgeCols.Count;)
                {
                    if (!Physics2D.GetIgnoreCollision(lineColls[i], nonLineColls[j]))
                        Physics2D.IgnoreCollision(lineColls[i], nonLineColls[j], true);
                    else
                    {
                        nonEdgeCols.RemoveAt(i);
                        j--;
                    }
                    j++;
                }
            }

            StartCoroutine(EndOfPhysics(lineColls, nonLineColls));
        }
    }

    private void AvoidCollisionWithLines()
    {
        Collider2D[] lineColls = new Collider2D[100];

        if (Physics2D.OverlapCollider(col, lineContacts, lineColls) > 0 )
        {

            List<EdgeCollider2D> edgeCols = new List<EdgeCollider2D>();
            List<List<Collider2D>> nonLineColsList = new List<List<Collider2D>>();

            foreach (var coll in lineColls)
            {
                edgeCols.Add(coll as EdgeCollider2D);
                Collider2D[] nonLineColls = new Collider2D[100];
                nonLineColsList.Add(new List<Collider2D>());
                if (Physics2D.OverlapCollider(col, nonLineContacts, nonLineColls) > 0)
                {
                    foreach (var colli in nonLineColls)
                    {
                        nonLineColsList[nonLineColsList.Count - 1].Add(colli);
                    }
                }
            }

            for (int i = 0; i < edgeCols.Count; i++)
            {
                for (int j = 0; j < nonLineColsList[i].Count;)
                {
                    if (!Physics2D.GetIgnoreCollision(lineColls[i], nonLineColsList[i][j]))
                        Physics2D.IgnoreCollision(lineColls[i], nonLineColsList[i][j], true);
                    else
                    {
                        nonLineColsList[i].RemoveAt(j);
                        j--;
                    }
                    j++;
                }
                StartCoroutine(EndOfPhysicsBis(edgeCols[i], nonLineColsList[i]));
            }

        }
    }

    IEnumerator EndOfPhysics(Collider2D[] lineColls, Collider2D[] nonLineColls)
    {
        yield return waiter;
        for (int i = 0; i < lineColls.Length; i++)
        {
            for (int j = 0; j < nonLineColls.Length; j++)
            {
                Physics2D.IgnoreCollision(lineColls[i], nonLineColls[j], false);
            }
        }
    }

    IEnumerator EndOfPhysicsBis(Collider2D lineColl, List<Collider2D> nonLineColls)
    {
        yield return waiter;
        for (int j = 0; j < nonLineColls.Count; j++)
        {
            Physics2D.IgnoreCollision(lineColl, nonLineColls[j], false);
        }
    }
}

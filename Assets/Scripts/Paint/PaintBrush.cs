using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintBrush : MonoBehaviour
{
    public float paintRange;
    public LayerMask paintLayers;

    public float brushWidth = 0.05f;
    public Texture2D brushTexture;
    public Texture2D eraserTexture;
    bool hitPaintArea;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    //void Update()
    //{
   //     UpdatePaint();
    //}

    public void UpdatePaint(bool erase, Vector2 pos)
    {
        //Raycast en avant
        Ray ray = new Ray(new Vector3(pos.x, pos.y, transform.position.z), transform.forward);
        RaycastHit hit;

        //Si on touche paint area 
        if (Physics.Raycast(ray, out hit, paintRange, paintLayers))
        {
            //print("Hit " + hit.collider.name);
            //Essayer de récupérer la paint Area
            PaintArea paintArea;
            if (!hit.collider.TryGetComponent<PaintArea>(out paintArea))
                return;

            //La notifier qu'elle est touché par le pinceau et en quel point ?
            if (!erase) paintArea.Paint(hit.textureCoord, brushWidth, brushTexture, erase);
            else paintArea.Paint(hit.textureCoord, brushWidth, eraserTexture, erase);
            hitPaintArea = true;
        }
        else
        {
            hitPaintArea = false;
        }
    }

    void OnDrawGizmos()
    {
        if (hitPaintArea)
            Gizmos.color = Color.blue;
        else
            Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * paintRange);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trajectory : MonoBehaviour
{
    [SerializeField] int dotsNumber;
    [SerializeField] GameObject dotsParent;
    [SerializeField] GameObject dotPrefab;
    [SerializeField] float dotSpacing;
    [SerializeField][Range(0.01f, 0.3f)] float dotMinScale;
    [SerializeField][Range(0.3f, 1f)] float dotMaxScale;
    [SerializeField] float airDrag = 3f;
    [SerializeField] float gravityScale = 1f;
    [SerializeField] private LayerMask m_WhatIsGround;

    Transform[] dotsList;
    Vector2 pos;

    float timeStamp;

    // Start is called before the first frame update
    void Start()
    {
        Hide();
        PrepareDots();
    }

    void PrepareDots()
    {
        dotsList = new Transform[dotsNumber];
        dotPrefab.transform.localScale = Vector3.one * dotMaxScale;

        float scale = dotMaxScale;
        float scaleFactor = scale / dotsNumber;

        for (int i = 0; i < dotsNumber; i++)
        {
            dotsList[i] = Instantiate(dotPrefab, null).transform;
            dotsList[i].parent = dotsParent.transform;
            dotsList[i].localScale = Vector3.one * scale;
            if (scale > dotMinScale)
                scale -= scaleFactor;
        }
    }

    public void UpdateDots(Vector2 forceApplied)
    {
        timeStamp = dotSpacing;
        float dragFactor;
        bool hitSpot = false;
        for (int i = 0; i < dotsNumber; i++)
        {
            if (!hitSpot)
            {
                dotsList[i].gameObject.SetActive(true);
                pos.x = transform.position.x + forceApplied.x * timeStamp;
                pos.y = transform.position.y + forceApplied.y * timeStamp - Physics2D.gravity.magnitude * gravityScale * timeStamp * timeStamp / 2f;
                dragFactor = (airDrag - airDrag * timeStamp * dotSpacing) / airDrag;
                pos *= dragFactor;
                dotsList[i].transform.position = pos;
                timeStamp += dotSpacing;

                Collider2D[] colliders = Physics2D.OverlapCircleAll(pos, dotsList[i].localScale.x, m_WhatIsGround);
                foreach (Collider2D c in colliders) 
                {
                    if (c.gameObject.tag == "Enemy" || c.gameObject.tag == "Wall" || c.gameObject.tag == "Ground" || c.gameObject.tag == "GroundNoSlide" || c.gameObject.tag == "Breakable Wall" || c.gameObject.tag == "obstacle")
                    {
                        hitSpot = true;
                        break;
                    }
                }
            }
            else
            {
                dotsList[i].gameObject.SetActive(false);
            }
        }
    }

    public void Show()
    {
        dotsParent.SetActive(true);
    }

    public void Hide()
    {
        dotsParent.SetActive(false);
    }
}

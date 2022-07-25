using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;
using DG.Tweening;
using System.Threading.Tasks;

public class GameControl : MonoBehaviour
{    
    [SerializeField] private Camera mainCam;
    
    
    private Collider2D collider2D;
    private Vector3 oldMousePos, currMousePos, deltaMousePos;
        
    private ObjectPooling panels_pool;
    private GameObject currentGameObjectPanel;

    private float coolDown = 0;
    private bool isBlockTouching = false;
    public const float DEF_COOLDOWN = 0.4f;
    public const int DEF_SIZE = 1;

    private int starting_x = -2;
    private int starting_y = 2;
    private int row_length = 5;

    private Dictionary<GameObject, Panel> panels = new Dictionary<GameObject, Panel>();

    // Start is called before the first frame update
    void Start()
    {
        panels_pool = new ObjectPooling(100, GameObject.Find("All panels").transform);
                
        for (int x = starting_x; x < (starting_x + row_length); x+= DEF_SIZE)
        {
            for (int y = starting_y; y > (starting_y - row_length); y-= DEF_SIZE)
            {
                AddPanel(new Vector3(x, y, 0));                
            }
        }

        CheckPanels();
    }

        // Update is called once per frame
    void Update()
    {
    
        if (coolDown > DEF_COOLDOWN && !isBlockTouching)
        {
            
            currMousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
            collider2D = Physics2D.OverlapPoint(currMousePos);

            if (collider2D != null)
            {
                

                if (Input.GetMouseButtonDown(0))
                {
                    if (collider2D.name == "restart")
                    {
                        isBlockTouching = false;
                        SceneManager.LoadScene("SampleScene");
                    }

                    currentGameObjectPanel = collider2D.gameObject;
                    oldMousePos = currMousePos;                    
                    deltaMousePos = Vector3.zero;
                    return;
                } 
                                
                
                if (currentGameObjectPanel == collider2D.gameObject)
                {
                    deltaMousePos = currMousePos - oldMousePos;
                    oldMousePos = currMousePos;                    
                }
                else
                {
                    currMousePos = Vector3.zero;
                    deltaMousePos = Vector3.zero;
                    oldMousePos = Vector3.zero;
                }


                if (Mathf.Abs(deltaMousePos.x) > 0 || Mathf.Abs(deltaMousePos.y) > 0)
                {
                    

                    if (Mathf.Abs(deltaMousePos.x) > Mathf.Abs(deltaMousePos.y)) //left right
                    {
                        if (deltaMousePos.x > 0) //right
                        {
                            MovePanelRight(currentGameObjectPanel);
                        }
                        else //left
                        {
                            MovePanelLeft(currentGameObjectPanel);
                        }
                    }
                    else
                    {
                        if (deltaMousePos.y > 0) //up
                        {
                            MovePanelDown(currentGameObjectPanel);
                        }
                        else //down
                        {
                            MovePanelUp(currentGameObjectPanel);
                        }
                    }

                    currentGameObjectPanel = null;
                }

            }

        }
        else
        {
            coolDown += Time.deltaTime;            
            oldMousePos = Vector3.one * 999;
        }
    }


    private void AddPanel(Vector3 position)
    {        
        GameObject go = panels_pool.GetObject();
        go.transform.position = position;
        go.SetActive(true);
        panels.Add(go, go.GetComponent<Panel>());
    }

    private void RemovePanel(List<GameObject> panelsToDel)
    {        
        for (int i = 0; i < panelsToDel.Count; i++)
        {            
            if (panelsToDel[i] == null) continue;
            panels.Remove(panelsToDel[i]);
            StartCoroutine(panelsToDel[i].GetComponent<Panel>().PlayDestroyEffect());
            panels_pool.ReturnObject(panelsToDel[i]);
        }
    }

    private void MovePanelLeft(GameObject _panel)
    {
        GameObject _secondPanel = WhatGameObjectInPosition(new Vector3(_panel.transform.position.x - DEF_SIZE, _panel.transform.position.y, 0));

        if (_secondPanel == null)
        {
            return;
        }

        Vector3 panelPos = _panel.transform.position;
        _panel.transform.DOMove(_secondPanel.transform.position, DEF_COOLDOWN);
        _secondPanel.transform.DOMove(panelPos, DEF_COOLDOWN);

        coolDown = 0;
        StartCoroutine(CheckPanelsAfterMove());
    }

    private void MovePanelRight(GameObject _panel)
    {
        GameObject _secondPanel = WhatGameObjectInPosition(new Vector3(_panel.transform.position.x + DEF_SIZE, _panel.transform.position.y, 0));

        if (_secondPanel == null)
        {
            return;
        }

        Vector3 panelPos = _panel.transform.position;
        _panel.transform.DOMove(_secondPanel.transform.position, DEF_COOLDOWN);
        _secondPanel.transform.DOMove(panelPos, DEF_COOLDOWN);

        coolDown = 0;
        StartCoroutine(CheckPanelsAfterMove());
    }

    private void MovePanelUp(GameObject _panel)
    {
        GameObject _secondPanel = WhatGameObjectInPosition(new Vector3(_panel.transform.position.x, _panel.transform.position.y- DEF_SIZE, 0));

        if (_secondPanel == null)
        {
            return;
        }

        Vector3 panelPos = _panel.transform.position;
        _panel.transform.DOMove(_secondPanel.transform.position, DEF_COOLDOWN);
        _secondPanel.transform.DOMove(panelPos, DEF_COOLDOWN);

        coolDown = 0;
        StartCoroutine(CheckPanelsAfterMove());
    }

    private void MovePanelDown(GameObject _panel)
    {
        GameObject _secondPanel = WhatGameObjectInPosition(new Vector3(_panel.transform.position.x, _panel.transform.position.y + DEF_SIZE, 0));

        if (_secondPanel == null)
        {
            return;
        }

        Vector3 panelPos = _panel.transform.position;
        _panel.transform.DOMove(_secondPanel.transform.position, DEF_COOLDOWN);
        _secondPanel.transform.DOMove(panelPos, DEF_COOLDOWN);

        coolDown = 0;
        StartCoroutine(CheckPanelsAfterMove());
    }

    private GameObject WhatGameObjectInPosition(Vector3 position)
    {
        GameObject result = null;

        foreach (GameObject items in panels.Keys)
        {
            if (items.transform.position == position) return items;
        }

        return result;
    }

    private Panel WhatPanelInPosition(Vector3 position)
    {
        Panel result = null;

        foreach (GameObject items in panels.Keys)
        {
            if (items.transform.position == position) return items.GetComponent<Panel>();
        }

        return result;
    }

    private IEnumerator CheckPanelsAfterMove()
    {
        yield return new WaitForSeconds(0.5f);
        CheckPanels();
    }


    private void CheckPanels()
    {
        List<GameObject> candidates = new List<GameObject>();
        bool isFound = false;

        for (int y = starting_y; y > (starting_y - row_length); y-= DEF_SIZE) 
        {
            int match = 1;
            for (int x = starting_x; x < (starting_x + row_length); x+= DEF_SIZE)
            {
                if (WhatPanelInPosition(new Vector3(x, y, 0))!=null && WhatPanelInPosition(new Vector3(x- DEF_SIZE, y, 0))!=null && WhatPanelInPosition(new Vector3(x, y, 0)).CurrentType == WhatPanelInPosition(new Vector3(x- DEF_SIZE, y, 0)).CurrentType)
                {
                    match++;
                    if (match == 2) candidates.Add(WhatGameObjectInPosition(new Vector3(x - DEF_SIZE, y, 0)));
                    candidates.Add(WhatGameObjectInPosition(new Vector3(x, y, 0)));                    
                }
                else
                {
                    if (match>=3)
                    {
                        RemovePanel(candidates);                       
                        isFound = true;
                    }
                    
                    match = 1;
                    candidates.Clear();
                }
            }

            if (match >= 3)
            {
                RemovePanel(candidates);
                isFound = true;
            }

            match = 1;
            candidates.Clear();           

        }

        if (isFound)
        {
            isBlockTouching = true;
            StartCoroutine(Relocation());
            return;
        }

        //vertical
        for (int x = starting_x; x < (starting_x + row_length); x+= DEF_SIZE) 
        {
            int match = 1;
            for (int y = starting_y; y > (starting_y - row_length); y-= DEF_SIZE)
            {
                if (WhatPanelInPosition(new Vector3(x, y, 0))!=null && WhatPanelInPosition(new Vector3(x, y+ DEF_SIZE, 0)) && WhatPanelInPosition(new Vector3(x, y, 0)).CurrentType == WhatPanelInPosition(new Vector3(x, y+ DEF_SIZE, 0)).CurrentType)
                {
                    match++;
                    if (match == 2) candidates.Add(WhatGameObjectInPosition(new Vector3(x, y+ DEF_SIZE, 0)));
                    candidates.Add(WhatGameObjectInPosition(new Vector3(x, y, 0)));
                }
                else
                {
                    if (match >= 3)
                    {
                        RemovePanel(candidates);
                        isFound = true;
                    }

                    match = 1;
                    candidates.Clear();
                }
            }

            if (match >= 3)
            {
                RemovePanel(candidates);
                isFound = true;
            }

            match = 1;
            candidates.Clear();

        }

        if (isFound)
        {
            isBlockTouching = true;
            StartCoroutine(Relocation());
            return;
        }
                
        isBlockTouching = false;
        
    }


    private IEnumerator Relocation()
    {       
        yield return new WaitForSeconds(Panel.DESTROY_SPEED);
        

        do
        {
            for (int x = starting_x; x < (starting_x + row_length); x+= DEF_SIZE)
            {
                if (WhatGameObjectInPosition(new Vector3(x, starting_y, 0)) == null)
                {
                    AddPanel(new Vector3(x, starting_y, 0));
                }
            }

            HashSet<GameObject> PanelsToMove = new HashSet<GameObject>();
            
            for (int x = starting_x; x < (starting_x + row_length); x+= DEF_SIZE)
            {
                for (int y = starting_y; y > (starting_y - row_length); y-= DEF_SIZE)
                {
                    if (WhatGameObjectInPosition(new Vector3(x, y, 0)) == null)
                    {
                        for (int i = starting_y; i > y; i-= DEF_SIZE)
                        {
                            if (WhatGameObjectInPosition(new Vector3(x, i, 0)) != null)
                            {
                                PanelsToMove.Add(WhatGameObjectInPosition(new Vector3(x, i, 0)));
                            }                            
                        }
                                                
                    }
                }
            }


            if (PanelsToMove.Count > 0)
            {
                foreach (GameObject item in PanelsToMove)
                {
                    item.transform.DOMove(new Vector3(item.transform.position.x, item.transform.position.y - DEF_SIZE, 0), Panel.MOVE_SPEED);
                }

                yield return new WaitForSeconds(0.5f);


            }
            else break;
        }
        while (true);

        Dictionary<int, int> SpawnNewPanels = new Dictionary<int, int>();
        for (int x = starting_x; x < (starting_x + row_length); x+= DEF_SIZE)
        {
            for (int y = starting_y; y > (starting_y - row_length); y-= DEF_SIZE)
            {
                if (WhatGameObjectInPosition(new Vector3(x, y, 0)) == null)
                {                    
                    if (!SpawnNewPanels.ContainsKey(x))
                    {
                        SpawnNewPanels.Add(x, 1);
                    }
                    else
                    {
                        SpawnNewPanels[x]++;
                    }
                }
            }
        }

        CheckPanels();
    }


}








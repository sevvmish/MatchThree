using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class GameControl : MonoBehaviour
{
    [SerializeField] private Button restartButton;
    [SerializeField] private Camera mainCam;
    [SerializeField] private TextMeshProUGUI texter;
        
    private RaycastHit2D hit;
    private Touch currentTouch;

    private ObjectPooling panels_pool;

    private float coolDown = 0;
    private bool isBlockTouching = true;
    public const float DEF_COOLDOWN = 0.5f;
    public const float DEF_LENGHT = 80;

    private float starting_x = -200;
    private float starting_y = 250;
    private int row_length = 6;

    private readonly HashSet<string> tagsForSimplePanels = new HashSet<string>() { "panel1", "panel2", "panel3", "panel4", "panel5" };
    private Dictionary<Vector2, Panel> panels = new Dictionary<Vector2, Panel>();

    // Start is called before the first frame update
    void Start()
    {
        panels_pool = new ObjectPooling(100, GameObject.Find("All panels").transform);

        float x = starting_x;
        float y = starting_y;
        int index = row_length;

        for (int i = 0; i < index; i++)
        {
            for (int ii = 0; ii < index; ii++)
            {
                GameObject p = panels_pool.GetObject();                
                Panel cur_Panel = p.GetComponent<Panel>();
                cur_Panel.SetPosition(new Vector2(x, y));
                panels.Add(new Vector2(x, y), cur_Panel);
                p.SetActive(true);
                x += DEF_LENGHT;
            }

            x = -200;
            y -= DEF_LENGHT;
        }

        CheckPanels();


        restartButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("SampleScene");
        });
    }

        // Update is called once per frame
    void Update()
    {        
        if (coolDown > DEF_COOLDOWN && !isBlockTouching)
        {
            hit = Physics2D.Raycast(Input.mousePosition, Vector2.zero);

            if ((Input.touchCount > 0 || Input.GetMouseButtonDown(0)) && hit.collider != null)
            {
                currentTouch = Input.GetTouch(0);

                if (currentTouch.phase == TouchPhase.Moved)
                {
                    texter.text = currentTouch.deltaPosition.ToString();
                    Panel p = hit.collider.GetComponent<Panel>();
                    if (Mathf.Abs(currentTouch.deltaPosition.x) > Mathf.Abs(currentTouch.deltaPosition.y)) //left right
                    {
                        //
                    }
                }
            }
        }
        else
        {
            coolDown += Time.deltaTime;
        }
    }

    private void CheckPanels()
    {
        

        try
        {
            List<Vector2> candidates = new List<Vector2>();
            bool isFound = false;

            for (float y = starting_y; y > (starting_y - DEF_LENGHT * row_length); y -= DEF_LENGHT)
            {
                int match = 1;
                for (float x = starting_x; x < (starting_x + DEF_LENGHT * row_length); x += DEF_LENGHT)
                {
                    //print(x +" - " + y);

                    if (panels.ContainsKey(new Vector2(x - DEF_LENGHT, y)) && panels[new Vector2(x, y)].CurrentType == panels[new Vector2(x - DEF_LENGHT, y)].CurrentType)
                    {
                        match++;
                        if (match == 2) candidates.Add(new Vector2(x - DEF_LENGHT, y));
                        candidates.Add(new Vector2(x, y));

                        //print(match + " : " + new Vector2(x, y));
                    }
                    else
                    {
                        if (match >= 3)
                        {
                            isFound = true;
                            break;
                        }
                        else
                        {
                            match = 1;
                            candidates.Clear();
                        }
                    }
                }

                if (match >= 3)
                {
                    isFound = true;
                    break;
                }
                else
                {
                    candidates.Clear();
                }
            }

            /*
            for (int i = 0; i < candidates.Count; i++)
            {
                print(candidates[i]);
            }
            */

            if (isFound)
            {
                StartCoroutine(DestroyMatchPanels(candidates));
                isBlockTouching = true;
                return;
            }

            //vertical
            candidates.Clear();
            isFound = false;

            for (float x = starting_x; x < (starting_x + DEF_LENGHT * row_length); x += DEF_LENGHT)
            {
                int match = 1;
                for (float y = starting_y; y > (starting_y - DEF_LENGHT * row_length); y -= DEF_LENGHT)
                {
                    //print(x +" - " + y);

                    if (panels.ContainsKey(new Vector2(x, y + DEF_LENGHT)) && panels[new Vector2(x, y)].CurrentType == panels[new Vector2(x, y + DEF_LENGHT)].CurrentType)
                    {
                        match++;
                        if (match == 2) candidates.Add(new Vector2(x, y + DEF_LENGHT));
                        candidates.Add(new Vector2(x, y));

                        //print(match + " : " + new Vector2(x, y));
                    }
                    else
                    {
                        if (match >= 3)
                        {
                            isFound = true;
                            break;
                        }
                        else
                        {
                            match = 1;
                            candidates.Clear();
                        }
                    }
                }

                if (match >= 3)
                {
                    isFound = true;
                    break;
                }
                else
                {
                    candidates.Clear();
                }
            }


            if (isFound)
            {
                StartCoroutine(DestroyMatchPanels(candidates));
                isBlockTouching = true;
                return;
            }

            isBlockTouching = false;
        }
        catch (System.Exception ex)
        {
            print("error" + ex);

            
        }
                
    }

    IEnumerator DestroyMatchPanels(List<Vector2> candidates)
    {
     
        if (candidates.Count>5)
        {
          
            for (int i = 0; i < (candidates.Count - 5); i++)
            {
                candidates.Remove(candidates[i + 5]);
            }
        }

        for (int i = 0; i < candidates.Count; i++)
        {
            StartCoroutine(panels[candidates[i]].DetonateMatch());            
        }

        yield return new WaitForSeconds(Panel.MOVE_SPEED);

        for (int i = 0; i < candidates.Count; i++)
        {            
            panels_pool.ReturnObject(panels[candidates[i]].gameObject);
            panels[candidates[i]] = null;
        }

        StartCoroutine(RenewMatchPanels(candidates));
    }

    IEnumerator RenewMatchPanels(List<Vector2> candidates)
    {
     
        //if vertical positions
        HashSet<float> x_variants = new HashSet<float>();
        HashSet<float> y_variants = new HashSet<float>();
        int vertPosRows = 0;
        for (int i = 0; i < candidates.Count; i++)
        {
            if (!x_variants.Contains(candidates[i].x))
            {
                x_variants.Add(candidates[i].x);
            }

            if (!y_variants.Contains(candidates[i].y))
            {
                y_variants.Add(candidates[i].y);
            }
        }

        vertPosRows = y_variants.Count;


        foreach (Vector2 items in panels.Keys)
        {
            if (x_variants.Contains(items.x) && items.y> y_variants.Max() && panels[items] != null)
            {
                panels[items].MoveDown(vertPosRows);
                
            }
        }

        yield return new WaitForSeconds(Panel.MOVE_SPEED);

        foreach (float x_coord in x_variants)
        {
            for (float y = starting_y; y > (starting_y - vertPosRows * DEF_LENGHT); y-=DEF_LENGHT)
            {
                GameObject p = panels_pool.GetObject();
                Panel cur_Panel = p.GetComponent<Panel>();
                cur_Panel.SetPosition(new Vector2(x_coord, y));
                
                p.SetActive(true);
            }
        }

        ReArrange();
     
    }


    private void ReArrange()
    {
        Dictionary<Vector2, Panel> newData = new Dictionary<Vector2, Panel>();

        /*
        foreach (Vector2 item in panels.Keys)
        {
            for (int i = 0; i < GameObject.Find("All panels").transform.childCount; i++)
            {
                if (GameObject.Find("All panels").transform.GetChild(i).gameObject.activeSelf)
                {
                    Panel p = GameObject.Find("All panels").transform.GetChild(i).GetComponent<Panel>();
                    if (p.GetPosition() == item)
                    {
                        newData.Add(item, p);
                        break;
                    }
                }
            }
        }
        */

        for (int i = 0; i < GameObject.Find("All panels").transform.childCount; i++)
        {
            if (GameObject.Find("All panels").transform.GetChild(i).gameObject.activeSelf)
            {
                Panel p = GameObject.Find("All panels").transform.GetChild(i).GetComponent<Panel>();
                newData.Add(new Vector2(Mathf.Round(GameObject.Find("All panels").transform.GetChild(i).GetComponent<RectTransform>().anchoredPosition3D.x), Mathf.Round(GameObject.Find("All panels").transform.GetChild(i).GetComponent<RectTransform>().anchoredPosition3D.y)), p);
            }
        }


        panels = newData;
        

        foreach (Vector2 item in panels.Keys)
        {
            if (panels[item] != null)
            {
                Vector2 v = new Vector2((int)panels[item].GetComponent<RectTransform>().anchoredPosition3D.x, (int)panels[item].GetComponent<RectTransform>().anchoredPosition3D.y);

                if (v != item)
                {
                    //print("errr1: " + item + " = " + v);
                    //ReArrange();
                }
            }
            else
            {
                //print("errr2: " + item);
                //ReArrange();
            }
        }

        CheckPanels();

    }


}








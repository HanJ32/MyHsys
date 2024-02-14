using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(ScrollRect))]
public class HsysCardFlipping : MonoBehaviour
{
    [Tooltip("边界范围")]
    public RectTransform Scrolling_Panel = new RectTransform();
    [Tooltip("事件队列")]
    public GameObject[] Array_Of_Event;
    [Tooltip("显示区域")]
    public RectTransform Center;
    [Tooltip("开始下标")]
    public int Starting_Index = -1;
    [Tooltip("内部按键 上部")]
    public GameObject Scroll_Up_Button;
    [Tooltip("内部按键 下部")]
    public GameObject Scroll_Down_Button;
    [Tooltip("按键事件")]
    public UnityEngine.Events.UnityEvent<int> Button_Clicked;

    //========================================================================
    private float[] m_dist_reposition;
    private float[] m_distance;
    private int m_min_elements_number;
    private int m_elements_length;

    private float deltaY;
    private string result;


    public HsysCardFlipping()
    {

    }

    public HsysCardFlipping(RectTransform scrollingPanel, GameObject[] arrayOfElements, RectTransform center)
    {
        Scrolling_Panel = scrollingPanel;
        Array_Of_Event = arrayOfElements;
        Center = center;
    }

    private void Awake()
    {
        //这里自己加载会失效 错误提示在 MenuItem 中给出
        ScrollRect scrollrect = this.GetComponent<ScrollRect>();
        if(!Scrolling_Panel)
        {
            Scrolling_Panel = scrollrect.content;
        }
        if(Center == null)
        {
            Debug.LogError("[Hsys Error] 没有定义 显示区域   HsysCardFlipping ==> RectTransform(Center)");
        }
        if(Array_Of_Event == null || Array_Of_Event.Length == 0)
        {
            int childcount = scrollrect.content.childCount;
            if(childcount > 0)
            {
                Array_Of_Event = new GameObject[childcount];
                for(int index = 0; index < childcount; index+=1)
                {
                    Array_Of_Event[index] = scrollrect.content.GetChild(index).gameObject;
                }
            }

        }
    }

    public void Start()
    {
        if (Array_Of_Event.Length < 1) { Debug.Log("没有子内容支持"); return; }
        m_elements_length = Array_Of_Event.Length;
        m_distance = new float[m_elements_length];
        m_dist_reposition = new float[m_elements_length];

        deltaY = Array_Of_Event[0].GetComponent<RectTransform>().rect.height * m_elements_length / 3 * 2;
        Vector2 startPosition = new Vector2(Scrolling_Panel.anchoredPosition.x, -deltaY);
        Scrolling_Panel.anchoredPosition = startPosition;

        for(int index = 0; index < m_elements_length; index+=1)
        {
            AddListener(Array_Of_Event[index], index);
        }
        if(Scroll_Up_Button)
        {
            Scroll_Up_Button.GetComponent<Button>().onClick.AddListener(() => { ScrollUp(); });
        }
        if(Scroll_Down_Button)
        {
            Scroll_Down_Button.GetComponent<Button>().onClick.AddListener(() => { ScrollDown(); });
        }
        if(Starting_Index > -1)
        {
            Starting_Index = Starting_Index > Array_Of_Event.Length ? Array_Of_Event.Length - 1 : Starting_Index;
            SnapToElement(Starting_Index);
        }
    }

    private void AddListener(GameObject button, int index)
    {
        button.GetComponent<Button>().onClick.AddListener(() => 
        {
            if(Button_Clicked != null)
            {
                Button_Clicked.Invoke(index);
            }
        });
    }

    public void Update()
    {
        if(Array_Of_Event == null || Array_Of_Event.Length < 1) { return; }

        for(int index = 0; index < m_elements_length; index+=1)
        {
            m_dist_reposition[index] = Center.GetComponent<RectTransform>().position.y - Array_Of_Event[index].GetComponent<RectTransform>().position.y;
            m_distance[index] = Mathf.Abs(m_dist_reposition[index]);

            float scale = Mathf.Max(0.7f, 1 / (1 + m_distance[index] /200));

            Array_Of_Event[index].GetComponent<RectTransform>().transform.localScale = new Vector3(scale, scale, 1f);
        }

        float mindistance = Mathf.Min(m_distance);

        for(int index = 0; index < m_elements_length; index+=1)
        {
            //将 Array_Of_Event 中的每个对象的 CanvasGroup 的交互作用关闭
            Array_Of_Event[index].GetComponent<CanvasGroup>().interactable = false;


            if(mindistance == m_distance[index])
            {
                m_min_elements_number = index;
                Array_Of_Event[index].GetComponent<CanvasGroup>().interactable = true;
                result = Array_Of_Event[index].GetComponentInChildren<TMP_Text>().text;
            }
        }
        ScrollingElements(-Array_Of_Event[m_min_elements_number].GetComponent<RectTransform>().anchoredPosition.y);
    }

    private void ScrollingElements(float _position)
    {
        float newY = Mathf.Lerp(Scrolling_Panel.anchoredPosition.y, _position, Time.deltaTime * 1.1f);
        Vector2 newpostion = new Vector2(Scrolling_Panel.anchoredPosition.x, newY);

        Scrolling_Panel.anchoredPosition = newpostion;
    }

    public string GetResults()
    {
        return result;
    }

    public void SnapToElement(int _element)
    {
        float deltaelementpositionY = Array_Of_Event[0].GetComponent<RectTransform>().rect.height * _element;
        Vector2 newposition = new Vector2(Scrolling_Panel.anchoredPosition.x, -deltaelementpositionY);
        Scrolling_Panel.anchoredPosition = newposition;
    }

    public void ScrollUp()
    {
        float deltaup = Array_Of_Event[0].GetComponent<RectTransform>().rect.height / 1.2f;
        Vector2 newpositionup = new Vector2(Scrolling_Panel.anchoredPosition.x, Scrolling_Panel.anchoredPosition.y - deltaup);
        Scrolling_Panel.anchoredPosition = Vector2.Lerp(Scrolling_Panel.anchoredPosition, newpositionup, 1);
    }

    public void ScrollDown()
    {
        float deltadown = Array_Of_Event[0].GetComponent<RectTransform>().rect.height / 1.2f;
        Vector2 newpositiondown = new Vector2(Scrolling_Panel.anchoredPosition.x, Scrolling_Panel.anchoredPosition.y + deltadown);
        Scrolling_Panel.anchoredPosition = newpositiondown;
    }
}

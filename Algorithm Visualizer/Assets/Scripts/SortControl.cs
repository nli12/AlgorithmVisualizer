using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SortControl : MonoBehaviour
{
    readonly int numberPos = 0;
    readonly int swapNoisePos = 1;
    readonly int correctNoisePos = 2;
    readonly int wrongNoisePos = 3;
    readonly int partitionNoisePos = 4;
    readonly int completeNoisePos = 5;
    readonly int checkNoisePos = 6;

    int count;
    float swapTime = 0.7f;

    public Transform templateCube;
    public string type;
    public bool interactive = false;

    int[] values;
    Transform[] cubes;

    Sequence sort;

    Queue<HashSet<int>> seq;

    int selectedOne = -1;
    int selectedTwo = -1;

    

    void Start()
    {

        count = 9;

        System.Random rnd = new System.Random();
        int lowerBound = 0;
        int upperBound = 100;

        sort = DOTween.Sequence();


        switch (type)
        {
            case "QuickSort":
                interactive = false;
                swapTime = 0.7f;
                break;
            case "BubbleSort":
                interactive = false;
                swapTime = 0.6f;
                break;
            case "BubbleInteract":
                interactive = true;
                swapTime = 0.6f;
                count = 5;
                break;
            default:
                break;
        }

        values = new int[count];
        cubes = new Transform[count];

        for (float x = count * -0.25f, i = 0; i < count; x += 0.5f, i++)
        {
            values[(int)i] = rnd.Next(lowerBound, upperBound);
            cubes[(int)i] = Instantiate(templateCube, new Vector3(x, 0, 4), Quaternion.identity);
            cubes[(int)i].GetComponent<Data>().index = (int) i;
            cubes[(int)i].GetChild(numberPos).GetComponent<TextMesh>().text = values[(int)i].ToString();
        }

        switch (type){
            case "QuickSort":
                QuickSort(0, values.Length - 1);
                CompletedAnimation();
                break;
            case "BubbleSort":
                BubbleSort();
                CompletedAnimation();
                break;
            case "BubbleInteract":
                BubbleSequence();
                BubbleSort();
                break;
            default:
                break;
        }
    }

    void CompletedAnimation()
    {
        foreach (Transform cube in cubes)
        {
            sort.Append(cube.GetComponent<Renderer>().material.DOColor(Color.white, 0.001f));
        }

        foreach (Transform cube in cubes)
        {
            sort.AppendCallback(() => { cube.GetChild(completeNoisePos).GetComponent<AudioSource>().Play(); });
            sort.Join(cube.GetComponent<Renderer>().material.DOColor(Color.green, 0.3f));
        }
    }

    public void BubbleSort()

    {
        foreach (Transform cube in cubes)
        {
            sort.Join(cube.GetComponent<Renderer>().material.DOColor(Color.yellow, 0.01f));
        }

        for (int i = 0; i < values.Length; i++)
        {
            for (int j = 0; j < values.Length - i - 1; j++)
            {

                sort.Append(cubes[j].GetComponent<Renderer>().material.DOColor(Color.red, 0.1f));

                //sort.AppendCallback(() => { cubes[j].GetChild(1).GetComponent<AudioSource>().Play();});

                sort.AppendInterval(0.3f);

                if (values[j] > values[j + 1])
                {
                    sort.Append(Swap(j, j + 1));
                    //Debug.Log(j + " " + (j + 1));
                }
                else
                {
                    sort.Append(cubes[j].GetComponent<Renderer>().material.DOColor(Color.yellow, 0.1f));
                }
            }
            sort.Append(cubes[values.Length - i - 1].GetComponent<Renderer>().material.DOColor(Color.green, 0.1f));
        }

    }

    private void BubbleSequence()
    {
        seq = new Queue<HashSet<int>>();
        int[] valuesDup = (int[])values.Clone();
        HashSet<int> temp;
        int tempValue;

        Debug.Log("Interact Sequence");

        for (int i = 0; i < valuesDup.Length; i++)
        {
            for (int j = 0; j < valuesDup.Length - i - 1; j++)
            {
                if (valuesDup[j] > valuesDup[j + 1])
                {
                    temp = new HashSet<int>();
                    temp.Add(j);
                    temp.Add(j + 1);
                    //Debug.Log(j + " " + (j+1));
                    seq.Enqueue(temp);

                    tempValue = valuesDup[j];
                    valuesDup[j] = valuesDup[j+1];
                    valuesDup[j+1] = tempValue;
                }
            }
        }

        /*
        foreach (int num in valuesDup)
        {
            Debug.Log(num);
        }
        */


    }

    public void selectCube(int index)
    {
        //var button = GameObject.FindGameObjectWithTag("Button");
        //TextMesh component = null;
        //if (button) component = (TextMesh)button.GetComponent<TextMesh>();
        // here we change the value of displayed text
        //if (component) component.text = index.ToString();
        //if (component) component.text = component.text + index.ToString();

        if (seq.Count == 0)
        {
            return; 
        }


        Sequence animation = DOTween.Sequence();
        animation.Append(cubes[index].GetComponent<Renderer>().material.DOColor(Color.yellow, 0.1f));
        

        if (selectedOne == index)
        {
            selectedOne = -1;
            selectedTwo = -1;
        }

        if (selectedOne == -1 && selectedTwo == -1)
        {
            selectedOne = index;

        } else if (selectedOne != -1 && selectedOne != index && selectedTwo == -1)
        {
            selectedTwo = index;
            if (seq.Peek().Contains(selectedOne) && seq.Peek().Contains(selectedTwo))
            {
                //Correct pair animation
                animation.AppendCallback(() => { cubes[selectedOne].GetChild(correctNoisePos).GetComponent<AudioSource>().Play(); });
                animation.Append(cubes[selectedOne].GetComponent<Renderer>().material.DOColor(Color.green, 0.1f));
                animation.Join(cubes[selectedTwo].GetComponent<Renderer>().material.DOColor(Color.green, 0.1f));
                animation.AppendInterval(0.5f);
                animation.Append(Swap(selectedOne, selectedTwo));
                animation.Append(cubes[selectedOne].GetComponent<Renderer>().material.DOColor(Color.white, 0.1f));
                animation.Join(cubes[selectedTwo].GetComponent<Renderer>().material.DOColor(Color.white, 0.1f));

                seq.Dequeue();
                if (seq.Count == 0)
                {
                    CompletedAnimation();
                }

            } else
            {
                //Wrong pair animation
                animation.AppendCallback(() => { cubes[selectedOne].GetChild(wrongNoisePos).GetComponent<AudioSource>().Play(); });
                animation.Append(cubes[selectedOne].GetComponent<Renderer>().material.DOColor(Color.red, 0.1f));
                animation.Join(cubes[selectedTwo].GetComponent<Renderer>().material.DOColor(Color.red, 0.1f));
                animation.AppendInterval(0.5f);
                animation.Append(cubes[selectedOne].GetComponent<Renderer>().material.DOColor(Color.white, 0.1f));
                animation.Join(cubes[selectedTwo].GetComponent<Renderer>().material.DOColor(Color.white, 0.1f));
            }
            selectedOne = -1;
            selectedTwo = -1;
        }
        selectedTwo = -1;
    }


    private void QuickSort(int i, int j)
    {
        if (i < j)
        {
            int pos = partition(i, j);
            QuickSort(i, pos - 1);
            QuickSort(pos + 1, j);
        }
    }

    private int partition(int i, int j)
    {
        int pivot = values[j];
        int small = i - 1;

       

        //Highlight the section of the array we're currently focusing on in yellow
        sort.Append(cubes[i].GetComponent<Renderer>().material.DOColor(Color.yellow, 0.01f));
        for (int a = i+1; a <= j; a++)
        {
            sort.Join(cubes[a].GetComponent<Renderer>().material.DOColor(Color.yellow, 0.01f));
        }

        //Highlight the pivot
        sort.AppendCallback(() => { cubes[j].GetChild(partitionNoisePos).GetComponent<AudioSource>().Play(); });
        sort.Append(cubes[j].GetComponent<Renderer>().material.DOColor(Color.magenta, 0.1f));

        for (int k = i; k < j; k++)
        {
            sort.Append(cubes[k].GetComponent<Renderer>().material.DOColor(Color.red, 0.1f));
            sort.AppendInterval(0.4f);
            if (values[k] <= pivot)
            {
                small++;
                if (k != small)
                {
                    
                    sort.Append(cubes[k].GetComponent<Renderer>().material.DOColor(Color.blue, 0.1f));
                    sort.Append(Swap(k, small));
                }
                else
                {
                    sort.Append(cubes[k].GetComponent<Renderer>().material.DOColor(Color.yellow, 0.1f));
                }

                sort.Append(cubes[small].GetComponent<Renderer>().material.DOColor(Color.blue, 0.1f));
            }
            else
            {
                sort.Append(cubes[k].GetComponent<Renderer>().material.DOColor(Color.yellow, 0.1f));
            }

        }
        if (j != small + 1)
        {
            sort.Append(Swap(j, small + 1));
            sort.Append(cubes[small + 1].GetComponent<Renderer>().material.DOColor(Color.yellow, 0.1f));
        }
        else
        {
            sort.Append(cubes[j].GetComponent<Renderer>().material.DOColor(Color.yellow, 0.1f));

        }

        //Clear all the colors
        sort.Append(cubes[i].GetComponent<Renderer>().material.DOColor(Color.white, 0.01f));
        for (int a = i + 1; a <= j; a++)
        {
            sort.Join(cubes[a].GetComponent<Renderer>().material.DOColor(Color.white, 0.01f));
        }

        return small + 1;
    }


    Sequence Swap(int a, int b)
    {

        int left, right;

        if (a < b)
        {
            left = a;
            right = b;
        }
        else
        {
            left = b;
            right = a;
        }

        Sequence cubeUp = DOTween.Sequence();
        cubeUp.AppendCallback(() => { cubes[left].GetChild(swapNoisePos).GetComponent<AudioSource>().Play(); });
        cubeUp.Join(cubes[left].DOMoveY(0.4f, swapTime));
        cubeUp.Append(cubes[left].DOMoveX((right - count / 2f) / 2, swapTime));
        cubeUp.Append(cubes[left].DOMoveY(0, swapTime));

        Sequence cubeDown = DOTween.Sequence();
        cubeDown.AppendCallback(() => { cubes[right].GetChild(swapNoisePos).GetComponent<AudioSource>().Play(); });
        cubeDown.Join(cubes[right].DOMoveY(-0.4f, swapTime));
        cubeDown.Append(cubes[right].DOMoveX((left - count / 2f) / 2, swapTime));
        cubeDown.Append(cubes[right].DOMoveY(0, swapTime));

        Sequence master = DOTween.Sequence();
        master.Join(cubeUp).Join(cubeDown);

        DataSwap(a, b);

        return master;

    }

    private void DataSwap(int a, int b)
    {
        Transform tempCube = cubes[a];
        cubes[a] = cubes[b];
        cubes[b] = tempCube;

        int tempValue = values[a];
        values[a] = values[b];
        values[b] = tempValue;

        cubes[a].GetComponent<Data>().index = a;
        cubes[b].GetComponent<Data>().index = b;
    }

    public void HoverUp(Transform cube)
    {
            Sequence seq = DOTween.Sequence();
            seq.Append(cube.DOMoveY(0.10f, 0.3f));
    }

    public void HoverDown(Transform cube)
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(cube.DOMoveY(0, 0.3f));
    }

    private void OnDestroy()
    {
        DOTween.Clear();
    }





}


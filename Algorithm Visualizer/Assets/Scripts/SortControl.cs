using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;

public class SortControl : MonoBehaviour
{
    readonly int numberPos = 0;
    readonly int swapNoisePos = 1;
    readonly int correctNoisePos = 2;
    readonly int wrongNoisePos = 3;
    readonly int partitionNoisePos = 4;
    readonly int completeNoisePos = 5;

    int count;
    float swapTime = 0.7f;
    float defaultY = 0.25f;

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
        //Set default number of cubes
        count = 7;

        sort = DOTween.Sequence();
        System.Random rnd = new System.Random();
        int lowerBound = 0;
        int upperBound = 100;

        //Set custom values for each scene
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
        float tempSize = 0f;
        int num = -1; 

        for (float x = count * -0.25f, i = 0; i < count; x += 0.5f, i++)
        {
            // Ensures there are no duplicate values 
            num = rnd.Next(lowerBound, upperBound);
            while (values.Contains(num))
            {
                num = rnd.Next(lowerBound, upperBound);
            }
            values[(int)i] = num;
            // Instantiates new cube
            cubes[(int)i] = Instantiate(templateCube, new Vector3(x, defaultY, 4), Quaternion.identity);
            // Sets cube value and index
            cubes[(int)i].GetComponent<Data>().index = (int)i;
            cubes[(int)i].GetChild(numberPos).GetComponent<TextMesh>().text = values[(int)i].ToString();
            // Sets cube size based on value
            tempSize = values[(int)i] / 600f + 0.15f;
            cubes[(int)i].localScale = new Vector3(tempSize, tempSize, tempSize);
        }

        switch (type){
            case "QuickSort":
                QuickSort(0, values.Length - 1);
                sort.Append(CompletedAnimation());
                break;
            case "BubbleSort":
                BubbleSort();
                sort.Append(CompletedAnimation());
                break;
            case "BubbleInteract":
                BubbleSequence();
                break;
            default:
                break;
        }
    }

    Sequence CompletedAnimation()
    {
        // Animation to be played when sort is completed
        Sequence animation = DOTween.Sequence();
        foreach (Transform cube in cubes)
        {
            animation.Append(cube.GetComponent<Renderer>().material.DOColor(Color.white, 0.001f));
        }

        foreach (Transform cube in cubes)
        {
            animation.AppendCallback(() => { cube.GetChild(completeNoisePos).GetComponent<AudioSource>().Play(); });
            animation.Join(cube.GetComponent<Renderer>().material.DOColor(Color.green, 0.3f));
        }
        return animation; 
    }

    public void BubbleSort()

    {
        // Set all cubes to yellow
        foreach (Transform cube in cubes)
        {
            sort.Join(cube.GetComponent<Renderer>().material.DOColor(Color.yellow, 0.01f));
        }

        // Execute bubble sort
        for (int i = 0; i < values.Length; i++)
        {
            for (int j = 0; j < values.Length - i - 1; j++)
            {
                // Set current cube being examined to red
                sort.Append(cubes[j].GetComponent<Renderer>().material.DOColor(Color.red, 0.1f));
                sort.AppendInterval(0.3f);

                // Swap with neighbor if greater than neighbor
                if (values[j] > values[j + 1])
                {
                    sort.Append(Swap(j, j + 1));
                }
                // Reset current cube to yellow if less than neighbor
                else
                {
                    sort.Append(cubes[j].GetComponent<Renderer>().material.DOColor(Color.yellow, 0.1f));
                }
            }

            // Set cube at end of iteration to green 
            sort.Append(cubes[values.Length - i - 1].GetComponent<Renderer>().material.DOColor(Color.green, 0.1f));
        }

    }

    // Create a queue of sets representing the swaps necessary to complete the bubble sort,
    // where each set contains the indices of the two elements that should be swapped at that step
    private void BubbleSequence()
    {
        
        int[] valuesDup = (int[])values.Clone();
        seq = new Queue<HashSet<int>>();
        HashSet<int> temp;
        int tempValue;

        for (int i = 0; i < valuesDup.Length; i++)
        {
            for (int j = 0; j < valuesDup.Length - i - 1; j++)
            {
                if (valuesDup[j] > valuesDup[j + 1])
                {
                    temp = new HashSet<int>();
                    temp.Add(j);
                    temp.Add(j + 1);
                    seq.Enqueue(temp);

                    tempValue = valuesDup[j];
                    valuesDup[j] = valuesDup[j+1];
                    valuesDup[j+1] = tempValue;
                }
            }
        }

    }

    public void selectCube(int index)
    {
        //var button = GameObject.FindGameObjectWithTag("Button");
        //TextMesh component = null;
        //if (button) component = (TextMesh)button.GetComponent<TextMesh>();
        // here we change the value of displayed text
        //if (component) component.text = index.ToString();
        //if (component) component.text = component.text + index.ToString();


        // Do nothing if the sort has been completed 
        if (seq.Count == 0)
        {
            return; 
        }


        Sequence animation = DOTween.Sequence();

        // Set currently selected cube to yellow 
        animation.Append(cubes[index].GetComponent<Renderer>().material.DOColor(Color.yellow, 0.1f));

        // If the same cube has been selected twice, reset both selections
        if (selectedOne == index)
        {
            selectedOne = -1;
            selectedTwo = -1;
            animation.Append(cubes[index].GetComponent<Renderer>().material.DOColor(Color.white, 0.1f));
        }
        // If no cubes have been selected, store the index of the first cube selected
        else if (selectedOne == -1 && selectedTwo == -1)
        {
            selectedOne = index;

        }
        // If the second cube is being selected, check if the pair indices are next in the swap sequence 
        else if (selectedOne != -1 && selectedOne != index && selectedTwo == -1)
        {
            selectedTwo = index;
            if (seq.Peek().Contains(selectedOne) && seq.Peek().Contains(selectedTwo))
            {
                //Correct pair animation
                animation.AppendCallback(() => { cubes[selectedOne].GetChild(correctNoisePos).GetComponent<AudioSource>().Play(); });
                animation.Join(cubes[selectedOne].GetComponent<Renderer>().material.DOColor(Color.green, 0.1f));
                animation.Join(cubes[selectedTwo].GetComponent<Renderer>().material.DOColor(Color.green, 0.1f));
                animation.AppendInterval(0.5f);
                animation.Append(Swap(selectedOne, selectedTwo));
                animation.Append(cubes[selectedOne].GetComponent<Renderer>().material.DOColor(Color.white, 0.1f));
                animation.Join(cubes[selectedTwo].GetComponent<Renderer>().material.DOColor(Color.white, 0.1f));

                seq.Dequeue();
                if (seq.Count == 0)
                {
                    animation.Append(CompletedAnimation());
                }

            } else
            {
                //Wrong pair animation
                animation.AppendCallback(() => { cubes[selectedOne].GetChild(wrongNoisePos).GetComponent<AudioSource>().Play(); });
                animation.Join(cubes[selectedOne].GetComponent<Renderer>().material.DOColor(Color.red, 0.1f));
                animation.Join(cubes[selectedTwo].GetComponent<Renderer>().material.DOColor(Color.red, 0.1f));
                animation.AppendInterval(0.8f);
                animation.Append(cubes[selectedOne].GetComponent<Renderer>().material.DOColor(Color.white, 0.1f));
                animation.Join(cubes[selectedTwo].GetComponent<Renderer>().material.DOColor(Color.white, 0.1f));
            }
            // Reset both selections 
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

        // Highlight the current partition in yellow
        sort.Append(cubes[i].GetComponent<Renderer>().material.DOColor(Color.yellow, 0.01f));
        for (int a = i+1; a <= j; a++)
        {
            sort.Join(cubes[a].GetComponent<Renderer>().material.DOColor(Color.yellow, 0.01f));
        }

        // Highlight the pivot
        sort.AppendCallback(() => { cubes[j].GetChild(partitionNoisePos).GetComponent<AudioSource>().Play(); });
        sort.Append(cubes[j].GetComponent<Renderer>().material.DOColor(Color.magenta, 0.1f));

        // Iterate through the current partition 
        for (int k = i; k < j; k++)
        {
            sort.Append(cubes[k].GetComponent<Renderer>().material.DOColor(Color.red, 0.1f));
            sort.AppendInterval(0.4f);
            // Highlight in blue if less than the pivot 
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
            // Set back to yellow if greater than the pivot 
            else
            {
                sort.Append(cubes[k].GetComponent<Renderer>().material.DOColor(Color.yellow, 0.1f));
            }

        }

        // Swap the pivot with the first element in the partition greater than the pivot 
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

        // Ensures the left cube always goes up and the right always goes down 
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

        // Generates swap animations 
        Sequence cubeUp = DOTween.Sequence();
        cubeUp.AppendCallback(() => { cubes[left].GetChild(swapNoisePos).GetComponent<AudioSource>().Play(); });
        cubeUp.Join(cubes[left].DOMoveY(defaultY + 0.33f, swapTime));
        cubeUp.Append(cubes[left].DOMoveX((right - count / 2f) / 2, swapTime));
        cubeUp.Append(cubes[left].DOMoveY(defaultY, swapTime));

        Sequence cubeDown = DOTween.Sequence();
        cubeDown.AppendCallback(() => { cubes[right].GetChild(swapNoisePos).GetComponent<AudioSource>().Play(); });
        cubeDown.Join(cubes[right].DOMoveY(defaultY - 0.33f, swapTime));
        cubeDown.Append(cubes[right].DOMoveX((left - count / 2f) / 2, swapTime));
        cubeDown.Append(cubes[right].DOMoveY(defaultY, swapTime));

        Sequence master = DOTween.Sequence();
        master.Join(cubeUp).Join(cubeDown);

        DataSwap(a, b);

        return master;

    }

    // Swaps the values in the underlying data structures 
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
        seq.Append(cube.DOMoveY(defaultY + 0.10f, 0.3f));
    }

    public void HoverDown(Transform cube)
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(cube.DOMoveY(defaultY, 0.3f));
    }

    public void PlayScene()  
    {
        DOTween.PlayAll();
    }

    public void PauseScene()
    {
        DOTween.PauseAll();
    }

    private void OnDestroy()
    {
        DOTween.Clear();
    }





}


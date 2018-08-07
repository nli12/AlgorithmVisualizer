using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Quick_Control : MonoBehaviour
{

    int count;

    public Transform templateCube;

    int[] values;
    Transform[] cubes;

    Sequence sort;

    void Start()
    {

        count = 10;

        System.Random rnd = new System.Random();
        int lowerBound = 0;
        int upperBound = 100;

        values = new int[count];
        cubes = new Transform[count];

        sort = DOTween.Sequence();


        for (float x = count * -0.25f, i = 0; i < count; x += 0.5f, i++)
        {
            values[(int)i] = rnd.Next(lowerBound, upperBound);
            cubes[(int)i] = Instantiate(templateCube, new Vector3(x, 0, 4), Quaternion.identity);
            cubes[(int)i].GetChild(0).GetComponent<TextMesh>().text = values[(int)i].ToString();
        }


        QuickSort(0, values.Length - 1);

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

        sort.Append(cubes[j].GetComponent<Renderer>().material.DOColor(Color.green, 0.1f));

        for (int k = i; k < j; k++)
        {
            sort.Append(cubes[k].GetComponent<Renderer>().material.DOColor(Color.red, 0.1f));
            if (values[k] <= pivot)
            {
                small++;
                if (k != small)
                {
                    sort.Append(Swap(k, small));
                    sort.Append(cubes[small].GetComponent<Renderer>().material.DOColor(Color.white, 0.1f));
                } else
                {
                    sort.Append(cubes[k].GetComponent<Renderer>().material.DOColor(Color.white, 0.1f));
                }
            } else
            {
                sort.Append(cubes[k].GetComponent<Renderer>().material.DOColor(Color.white, 0.1f));
            }
            
        }
        if (j != small + 1)
        {
            sort.Append(Swap(j, small + 1));
            sort.Append(cubes[small + 1].GetComponent<Renderer>().material.DOColor(Color.white, 0.1f));
        } else
        {
            sort.Append(cubes[j].GetComponent<Renderer>().material.DOColor(Color.white, 0.1f));

        }

        

        return small + 1;
    }


   

    Sequence Swap(int a, int b)
    {

        int left, right;
        float time = 0.75f;

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
        cubeUp.Append(cubes[left].DOMoveY(0.4f, time));
        cubeUp.Append(cubes[left].DOMoveX((right - count / 2f) / 2, time));
        cubeUp.Append(cubes[left].DOMoveY(0, time));

        Sequence cubeDown = DOTween.Sequence();
        cubeDown.Append(cubes[right].DOMoveY(-0.4f, time));
        cubeDown.Append(cubes[right].DOMoveX((left - count / 2f) / 2, time));
        cubeDown.Append(cubes[right].DOMoveY(0, time));

        Sequence master = DOTween.Sequence();
        master.Join(cubeUp).Join(cubeDown);

        DataSwap(a, b);

        return master;

    }

    void DataSwap(int a, int b)
    {
        Transform tempCube = cubes[a];
        cubes[a] = cubes[b];
        cubes[b] = tempCube;

        int tempValue = values[a];
        values[a] = values[b];
        values[b] = tempValue;
    }

    private void OnDestroy()
    {
        DOTween.Clear();
    }





}

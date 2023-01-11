using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Barracuda;

public class load_onnx : MonoBehaviour
{    
    public NNModel modelAsset;
    private Model runtimeModel;
    private IWorker worker;
    private string outputLayer;

    [SerializeField]
    private TMP_InputField inputValue;
    [SerializeField]
    private TMP_Text outputPrediction;

    // Start is called before the first frame update
    void Start()
    {
        runtimeModel = ModelLoader.Load(modelAsset);     
        worker = WorkerFactory.CreateWorker(WorkerFactory.Type.Auto, runtimeModel);
        outputLayer = runtimeModel.outputs[runtimeModel.outputs.Count - 1];

    }

    public void Predict()
    {
        int numberInput; 
        if(int.TryParse(inputValue.text, out numberInput)) // text가 int값을 정상적으로 가지고 있다면 저장.  
        {
            using Tensor inputTensor = new Tensor (1,1);
            inputTensor[0] = numberInput;
            worker.Execute(inputTensor);
            
            // outputLayer = runtimeModel.outputs[runtimeModel.outputs.Count - 1];
            Tensor outputTensor = worker.PeekOutput(outputLayer); 
            outputPrediction.text = outputTensor[0].ToString();
        }
    }

    public void OnDestroy()
    {
        worker?.Dispose();
    }

    // Update is called once per frame
    // void Update()
    // {
    // }
}

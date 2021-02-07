using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private ThreadWorker worker = null;

    void Start() {
        worker = new ThreadWorker();
        worker.Start(test());
        worker.Resume();
    }

    // Update is called once per frame
    void Update() {
    }

    IEnumerator test() {
        while (true) {
            Debug.Log("Test...");
            yield return new WaitForSeconds(2f);
        }
    }

    void OnDestroy() {
        if(worker != null)
            worker.Abort();
    }
}


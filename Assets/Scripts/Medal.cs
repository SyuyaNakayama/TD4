using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Medal : MonoBehaviour
{
    private MedalCounter saveMedals;
    //???????
    public int medalNum;

    // Start is called before the first frame update
    void Start()
    {
        GameObject managerObject = GameObject.Find("GameManager");
        saveMedals = managerObject.GetComponent<MedalCounter>();
    }

    // Update is called once per frame
    void Update()
    {
        if (saveMedals.GetMedalData(medalNum))
        {
            //Destroy(gameObject);
        }
        transform.Rotate(new Vector3(90, 0, 0) * Time.deltaTime);
    }

    public void OnTriggerEnter(Collider col)
    {
        LiveEntity liveEntity =
            col.gameObject.GetComponent<LiveEntity>();
        if (liveEntity && liveEntity.GetUserControl())
        {
            saveMedals.AddMedalCount();
            saveMedals.AcquisitionMedal(medalNum);
            Destroy(gameObject);
        }
    }
}

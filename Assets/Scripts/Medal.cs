using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Medal : MonoBehaviour
{
    [SerializeField]
    GameObject particle;
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
    }

    public void OnTriggerEnter(Collider col)
    {
        LiveEntity liveEntity =
            col.gameObject.GetComponent<LiveEntity>();
        if (liveEntity && liveEntity.GetUserControl())
        {
            saveMedals.AddMedalCount();
            saveMedals.AcquisitionMedal(medalNum);

            GameObject tempParticle =
                Instantiate(particle, transform.position, transform.rotation);
            tempParticle.transform.localScale = transform.lossyScale;

            Destroy(gameObject);
        }
    }
}

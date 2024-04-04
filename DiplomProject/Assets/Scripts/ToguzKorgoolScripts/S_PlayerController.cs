using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_PlayerController : MonoBehaviour
{
    public LayerMask layer;
    public List<GameObject> OurLunki;
    private S_Lunka ClickedLunka;
    public GameObject OurScoreLunka;
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            ClickLunka();
        }
    }

    void ClickLunka()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit, 1000f, layer))
        {
                GameObject hitObject = hit.collider.gameObject;
                
                if (hitObject != null && OurLunki.Contains(hitObject))
                {

                    // Объект содержится в списке OurLunki
                    ClickedLunka = hitObject.GetComponent<S_Lunka>();
                    // Debug.Log("Object is in OurLunki list.");
                    hitObject.GetComponent<S_Lunka>().OnClicked();

                    
                    S_Lunka Lunka=ClickedLunka.NextLunka;
                    if (ClickedLunka.KorgoolsCount==1)
                    {
                        Lunka.AddKorgool();
                        
                        Debug.Log(Lunka.index+" and "+Lunka.KorgoolsCount);
                    }
                    else{
                        for(int i=0; i<ClickedLunka.KorgoolsCount-1; i++)
                        {
                            Lunka.AddKorgool();
                            
                            Debug.Log(Lunka.index+" and "+Lunka.KorgoolsCount);

                            if(i<ClickedLunka.KorgoolsCount-2)
                            {
                                Lunka = Lunka.NextLunka;
                            }
                        }
                    }
                    if(!OurLunki.Contains(Lunka.gameObject) && Lunka.KorgoolsCount%2==0)
                    {
                        OurScoreLunka.GetComponent<S_Schetchik>().ApplyScore(Lunka.KorgoolsCount);
                        Lunka.Clear();//Убираем чтоб добавились очки
                    }
                    ClickedLunka.Remove();
                }
                else
                {
                    // Объект не содержится в списке OurLunki
                    Debug.Log("Object is not in OurLunki list.");
                }
        }
    }
}

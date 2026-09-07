using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pot : MonoBehaviour
{
    private Animator anim;
    [SerializeField] private LootTable lootTable;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void Smash(){
        anim.SetBool("smash", true);
        StartCoroutine(breakCo());
    }

    IEnumerator breakCo(){
        yield return new WaitForSeconds(.3f);
        DropLoot();
        this.gameObject.SetActive(false);
    }

    private void DropLoot()
    {
        if (lootTable == null) return;
        PowerUp loot = lootTable.LootPowerUp();
        if (loot != null)
        {
            Instantiate(loot.gameObject, transform.position, Quaternion.identity);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
public class ToDark : MonoBehaviour
{
    public Image image;
    public string ChangedNextScene;
    // Start is called before the first frame update

    // Update is called once per frame
    public void LetDarkness()
    {
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        Color color = image.color;
        image.transform.SetAsLastSibling();

        while (color.a < 1.0f)
        {
            color.a += 0.01f;
            yield return new WaitForSeconds(0.01f);
            image.color = new Color(0, 0, 0, color.a);

            if (color.a > 1.0f)
            {
                SceneManager.LoadScene(ChangedNextScene); //"IntroCutScene"
            }
        }

    }
    
}

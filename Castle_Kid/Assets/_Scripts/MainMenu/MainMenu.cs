using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Scripts.MainMenu
{
    public class MainMenu : MonoBehaviour
    {
        public GameObject backgroundPanel;

        private void Start()
        {
            backgroundPanel = GameObject.Find("BackgroundPanel"); 
        }

        public void StartGame()
        {
            try 
            {
                (backgroundPanel ?? GameObject.Find("BackgroundPanel"))?.SetActive(false);
            }
            catch
            {
                //rien
            }

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    
    }
}

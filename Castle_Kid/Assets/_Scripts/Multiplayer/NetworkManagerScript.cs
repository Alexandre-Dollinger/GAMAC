using System.Net;
using System.Net.Sockets;
using _Scripts.GameManager;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

namespace _Scripts.Multiplayer
{
    public class NetworkManagerScript : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI ipAddressText;
        [SerializeField] private TMP_InputField ip;
        [SerializeField] private Canvas multiplayerCanvas;
        
        private string _ipAddress;
        private UnityTransport _transport;
        
        void Start()
        {
            multiplayerCanvas.gameObject.SetActive(true);
            _ipAddress = "0.0.0.0";
            SetIpAddress(); // Set the Ip to the above address
        }
        
        // To Host a game
        public void StartHost() {
            NetworkManager.Singleton.StartHost();
            GetLocalIPAddress();
            GM.GameStarted = true;
        }
        
        // To Join a game
        public void StartClient() {
            if (ip.text == "")
            {
                Debug.Log("Please enter an IP that you want to Join");
                return;
            }
            _ipAddress = ip.text;
            ipAddressText.text = "IP : " + _ipAddress;
            SetIpAddress();
            NetworkManager.Singleton.StartClient();
            GM.GameStarted = true;
        }
        
        private void SetIpAddress() {
            _transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            _transport.ConnectionData.Address = _ipAddress;
        }
        
        private string GetLocalIPAddress() {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress eachIP in host.AddressList) {
                if (eachIP.AddressFamily == AddressFamily.InterNetwork) {
                    ipAddressText.text = "IP : " + eachIP;
                    _ipAddress = eachIP.ToString();
                    return eachIP.ToString();
                }
            }
            throw new System.Exception("No network adapters with an IPv4 address in the system!");
        }
    }
}

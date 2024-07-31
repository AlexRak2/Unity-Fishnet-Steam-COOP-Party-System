using System.Collections.Generic;
using System.Threading.Tasks;
using Steamworks;
using Steamworks.Data;
using UnityEngine;
using Color = UnityEngine.Color;

namespace Game.Utils
{
    public static class SteamExtension
    {
        public static Texture2D GetTextureFromImage(Steamworks.Data.Image image)
        {
            Texture2D texture = new Texture2D((int)image.Height, (int)image.Width);

            for (int x = 0; x < image.Width; x++)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    var pixel = image.GetPixel(x, y);
                    texture.SetPixel(x, (int)image.Height - y, new Color(pixel.r / 255f, pixel.g / 255f, pixel.b / 255f, pixel.a / 255f));
                }
            }
            
            texture.Apply();
            return texture;
        }
        
        public static async Task<List<Lobby>> GetLobbies()
        {
            Lobby[] lobbyArray = await SteamMatchmaking.LobbyList.WithKeyValue("Filter", "true").RequestAsync();
            List<Lobby> lobbies = new List<Lobby>();

            if (lobbyArray != null)
            {
                foreach (var lobby in lobbyArray)
                {
                    lobbies.Add(lobby);
                }
            }

            return lobbies;
        }
    }
}
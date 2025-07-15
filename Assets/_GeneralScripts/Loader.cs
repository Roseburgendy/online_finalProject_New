using UnityEngine.SceneManagement;
using Photon.Pun;

public static class Loader {

    public enum Scene {
        TitleScene,
        MainMenuScene,
        GameScene,
        LoadingScene,
        LobbyScene,
        //CharacterSelectScene,
        LXXCharacterSelectScene
    }

    private static Scene targetScene;
    public static void Load(Scene targetScene) {
        Loader.targetScene = targetScene;
        SceneManager.LoadScene(Scene.LoadingScene.ToString());
    }
    
    public static void LoadNetwork(Scene targetScene) {
        PhotonNetwork.LoadLevel(targetScene.ToString());
    }
    public static void LoaderCallback() {
        SceneManager.LoadScene(targetScene.ToString());
    }
}
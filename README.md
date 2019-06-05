# Asset Bundle Simplified

The purpose of this package is to simplify the usage of Asset Bundles. It's main features are:

* Dynamic loading of asset bundle dependencies (through a manifest).
* Dependency counter to correctly unload shared dependent asset bundles.
* "Simulation Mode", to quickly test modifications without rebuilding the asset bundle.
* Debug window to track which asset bundles are loaded.

## Basic Usage

### Load Assets

When you load an asset, it's bundle is automatically loaded. You must unload this bundle manually.
The following code shows how to load a prefab and instantiate it:

```csharp
private void CreateObject()
{
    GameObject prefab = BundleResources.LoadAsset<GameObject>("bundleName", "prefabName");
    Instantiate(prefab);
}
```

It is also possible to load all assets from a given type:

```csharp
private YourConfig[] LoadConfigs()
{
    YourConfig[] configs = BundleResources.LoadAllAssets<YourConfig>("bundleName");
    return configs;
}
```

### Load Assets Asynchronously

The following code shows how to load a prefab and instantiate it, using an asynchronous coroutine:

```csharp
private IEnumerator Start()
{
    var request = BundleResources.LoadAssetAsync<GameObject>("bundleName", "prefabName");
    yield return request; // Waits until the asset is loaded.
    Instantiate(request.Asset);
}
```

You can also register a callback that is called when the asset finishes loading.

```csharp
private void Start()
{
    var request = BundleResources.LoadAssetAsync<GameObject>("bundleName", "prefabName");
    request.OnComplete += (asset) => {
        Instantiate(asset);
    }
}
```

The API returns an object similar to the native Unity AsyncOperations.

### Load Scenes

The following code shows how to load scenes from an AssetBundle.

```csharp
private void LoadMenu()
{
    BundleResources.LoadScene("bundleName", "sceneName", LoadSceneMode.Single);
}
```

You may load them in any LoadSceneMode.

### Unload Asset Bundles

The following code shows how to unload an asset bundle.

```csharp
private void UnloadBundle()
{
    BundleResources.UnloadBundle("bundleName")
}
```

## Build Asset Bundles

To use the API, the asset bundles should be built on the path `StreamingAssets/bundles`.
   
```csharp
[MenuItem("Tools/Asset Bundle Resources/Build Asset bundles")]
private static void BuildAssetBundles()
{
    string assetBundlesPath = Path.Combine(Application.streamingAssetsPath, BundleResources.PATH_IN_STREAMING_ASSETS);

    assetBundlesPath = assetBundlesPath.Replace(@"\", "/");
    
    if (Directory.Exists(assetBundlesPath))
    {
        Directory.Delete(assetBundlesPath, true);
    }

    Directory.CreateDirectory(assetBundlesPath);

    BuildPipeline.BuildAssetBundles(assetBundlesPath,
        BuildAssetBundleOptions.ChunkBasedCompression
        | BuildAssetBundleOptions.ForceRebuildAssetBundle, EditorUserBuildSettings.activeBuildTarget);

    AssetDatabase.Refresh();
}
```

## Organização de Asset Bundles

Uma restrição para o uso dessa API é a forma como os bundles devem ser organizados. Dois tipos de bundles podem ser utilizados:

* Bundles "raiz": São bundles que o jogo usa diretamente através da API, geralmente são bundles que possuem cenas, 
prefabs ou arquivos de configurações.
* Bundles filhos: São bundles que contém os assets utilizados por bundles raiz, geralmente contém texturas, 
malhas, arquivos de animação, etc. Esses bundles são carregados indiretamente quando um bundle raiz é carregado e depende de um deles. **A API não dá suporte a carregar bundles filhos diretamente e não deve funcionar corretamente nesse caso!**


Essa organização permite que através de um mecanismo de contagem de referências os bundles filhos sejam descarregados automaticamente
 quando não estejam mais sendo utilizados, dando maior flexibilidade e permitindo o compartilhamente de assets entre diferentes bundles raiz.

## Modo de simulação

O modo de simulação utiliza o AssetDatabase para simular o uso da API no editor, permitindo que mudanças de assets sejam testadas mais facilmente sem a necessidade de fazer builds de novos bundles.

Para ligar e desligar o modo de simulação, basta ir no menu **Tools/Asset Bundle Simplified** e ligar/desligar o toogle de "Simulation Mode"

## Debug Window

A janela de debug permite visualizar em tempo real os bundles que estão carregados durante a execução do jogo **no editor**.
Também é possível verificar a contagem de referências para cada bundle filho.
Para abrir a janela de debug basta clicar em **Tools/Asset Bundle Simplified/DebugWindow**

```csharp
private void CreateObject()
{
    GameObject prefab = BundleResources.LoadAsset<GameObject>("bundleName", "prefabName");
    Instantiate(prefab);
}
```
    
Também é possível carregar todos os assets de um determinado tipo:

```csharp
private DecorationConfig[] LoadConfigs()
{
    DecorationConfig[] configs = BundleResources.LoadAllAssets<DecorationConfig>("bundleName");
    return configs;
}
```
    
Note que ao carregar um asset, o bundle é automaticamente carregado. Sendo necessário descarregar manualmente após o uso (veja a seguir).

### Carregando assets assincronamente
 
O exemplo a seguir mostrar como carregar um asset de forma assincrona em uma corotina:

```csharp
private IEnumerator Start()
{
    var request = BundleResources.LoadAssetAsync<GameObject>("bundleName", "prefabName");
    yield return request; //Espera o carregamento do asset
    Instantiate(request.Asset);
}
```
    
Também é possível utilizar um callback para executar ações após o carregamento do asset.

```csharp
private void Start()
{
    var request = BundleResources.LoadAssetAsync<GameObject>("bundleName", "prefabName");
    request.OnComplete += (asset) => {
        Instantiate(asset);
    }
}
```

Note que a API retorna um objeto similar às AsyncOperations nativas da Unity.

### Carregar cenas

O exemplo a seguir mostra como carregar cenas contidas em um AssetBundle.

```csharp
private void LoadMenu()
{
    BundleResources.LoadScene("bundleName", "sceneName", LoadSceneMode.Single);
}
```
    
Note que é possível carregar cenas de forma single ou aditivas.

### Descarregar bundles

O exemplo a seguir mostrar como descarregar um bundle após o uso.

```csharp
private void UnloadBundle()
{
    BundleResources.UnloadBundle("bundleName")
}
```

## Build de Asset Bundles

To use the API, it is necessary to build the asset bundles in the path StreamingAssets/bundles.
   
```csharp
[MenuItem("Tools/Asset Bundle Simplified/Build Asset Bundles")]
private static void BuildAssetBundles()
{
    string assetBundlesPath = Path.Combine(Application.streamingAssetsPath, BundleResources.PATH_IN_STREAMING_ASSETS);

    assetBundlesPath = assetBundlesPath.Replace(@"\", "/");
    
    if (Directory.Exists(assetBundlesPath))
    {
        Directory.Delete(assetBundlesPath, true);
    }

    Directory.CreateDirectory(assetBundlesPath);

    BuildPipeline.BuildAssetBundles(assetBundlesPath,
        BuildAssetBundleOptions.ChunkBasedCompression
        | BuildAssetBundleOptions.ForceRebuildAssetBundle, EditorUserBuildSettings.activeBuildTarget);

    AssetDatabase.Refresh();
}
```

## Asset Bundle declarations

To use this API the bundles must be organized in a specific way. There are two kinds of bundles:

* "Root" Asset Bundles: These bundles are directly used through the API. They usually contain scenes, prefabs or configs.
* "Child" Asset Bundles: These bundles contain assets used by *Root Asset Bundles*, like textures, meshes, animations, etc. They are loaded automatically when a *root bundle* that depends on it is loaded. **Loading these bundles directly will cause undefined behaviors.**

The API keeps track of how many *root bundles* depend on each *child bundle*, and unloads them when it is no longer being used, allowing for easy sharing between bundles.

## Simulation Mode

The simulation mode allows the usage of asset bundles in the **editor** without building them first.

You can toggle this feature at *Tools/Asset Bundle Simplified/SimulationModes*.

## Debug Window

The window allows you to keep track of all loaded bundles during the game execution in the editor. It also shows the reference count for the *child bundles*.

Open the window at *Tools/Asset Bundle Simplified/DebugWindow*.

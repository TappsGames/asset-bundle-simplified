#Asset Bundle Tools

Bundle Resources é uma API para simplificar o uso de asset bundles. Suas principais features são:

* Load automatico de dependências de asset bundles (através de um manifesto)
* Contagem de referências para descarregar corretamente dependências compartilhadas entre multiplos bundles
* Modo de "simulação", permitindo testar rapidamente modificações em assets sem precisar fazer builds de Assets Bundles.
* Janela de debug para permitir visualizar facilmente quais bundles estão carregados.

## Uso básico da API

### Carregando assets 
O trecho a seguir exemplifica como carregar um prefab e instanciar.
#Asset Bundle Tools

Bundle Resources é uma API para simplificar o uso de asset bundles. Suas principais features são:

* Load automatico de dependências de asset bundles (através de um manifesto)
* Contagem de referências para descarregar corretamente dependências compartilhadas entre multiplos bundles
* Modo de "simulação", permitindo testar rapidamente modificações em assets sem precisar fazer builds de Assets Bundles.
* Janela de debug para permitir visualizar facilmente quais bundles estão carregados.

## Uso básico da API

### Carregando assets 
O trecho a seguir exemplifica como carregar um prefab e instanciar.

```csharp
private void CreateObject()
{
    GameObject prefab = BundleResources.LoadAsset<GameObject>("nomeDoBundle", "nomeDoPrefab");
    Instantiate(prefab);
}
```
    
Também é possível carregar todos os assets de um determinado tipo:

```csharp
private DecorationConfig[] LoadConfigs()
{
    DecorationConfig[] configs = BundleResources.LoadAllAssets<DecorationConfig>("nomeDoBundle");
    return configs;
}
```

Note que ao carregar um asset, o bundle é automaticamente carregado. Sendo necessário descarregar manualmente após o uso (veja a seguir).

### Carregando assets assincronamente
 
O exemplo a seguir mostrar como carregar um asset de forma assincrona em uma corotina:

```csharp
private IEnumerator Start()
{
    var request = BundleResources.LoadAssetAsync<GameObject>("nomeDoBundle", "nomeDoPrefab");
    yield return request; //Espera o carregamento do asset
    Instantiate(request.Asset);
}
```
    
Também é possível utilizar um callback para executar ações após o carregamento do asset.

```csharp
private void Start()
{
    var request = BundleResources.LoadAssetAsync<GameObject>("nomeDoBundle", "nomeDoPrefab");
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
    BundleResources.LoadScene("nomeDoBundle", "nomeDaCena", LoadSceneMode.Single);
}
```
    
Note que é possível carregar cenas de forma single ou aditivas.

### Descarregar bundles

O exemplo a seguir mostrar como descarregar um bundle após o uso.

```csharp
private void UnloadBundle()
{
    BundleResources.UnloadBundle("nomeDoBundle")
}
```

## Build de Asset Bundles

Para utilizar a API, é necessário que os bundles do projeto sejam compilados e incluídos na pasta StreamingAssets/bundles do projeto. 
Uma forma comum de fazer esse processo é utilizando um script de CustomPreBuild para o processo de build no Go, como o exemplo a seguir:

```csharp
public class CustomPreBuild
{
    public static void PreBuildProcess()
    {
        BuildAssetBundles();

        AssetDatabase.Refresh();

        EditorApplication.Exit(0);
    }

    [MenuItem("Tools/Asset Bundle Simplified/Build Asset bundles")]
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

Para ligar e desligar o modo de simulação, basta ir no menu Tools/Asset Bundle Simplified e ligar/desligar o toogle de "Simulation Mode"

##Debug Window

A janela de debug permite visualizar em tempo real os bundles que estão carregados durante a execução do jogo **no editor**.
Também é possível verificar a contagem de referências para cada bundle filho.
Para abrir a janela de debug basta clicar em **Tools/Asset Bundle Simplified/DebugWindow**

```csharp
private void CreateObject()
{
    GameObject prefab = BundleResources.LoadAsset<GameObject>("nomeDoBundle", "nomeDoPrefab");
    Instantiate(prefab);
}
```
    
Também é possível carregar todos os assets de um determinado tipo:

```csharp
private DecorationConfig[] LoadConfigs()
{
    DecorationConfig[] configs = BundleResources.LoadAllAssets<DecorationConfig>("nomeDoBundle");
    return configs;
}
```
    
Note que ao carregar um asset, o bundle é automaticamente carregado. Sendo necessário descarregar manualmente após o uso (veja a seguir).

### Carregando assets assincronamente
 
O exemplo a seguir mostrar como carregar um asset de forma assincrona em uma corotina:

```csharp
private IEnumerator Start()
{
    var request = BundleResources.LoadAssetAsync<GameObject>("nomeDoBundle", "nomeDoPrefab");
    yield return request; //Espera o carregamento do asset
    Instantiate(request.Asset);
}
```
    
Também é possível utilizar um callback para executar ações após o carregamento do asset.

```csharp
private void Start()
{
    var request = BundleResources.LoadAssetAsync<GameObject>("nomeDoBundle", "nomeDoPrefab");
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
    BundleResources.LoadScene("nomeDoBundle", "nomeDaCena", LoadSceneMode.Single);
}
```
    
Note que é possível carregar cenas de forma single ou aditivas.

### Descarregar bundles

O exemplo a seguir mostrar como descarregar um bundle após o uso.

```csharp
private void UnloadBundle()
{
    BundleResources.UnloadBundle("nomeDoBundle")
}
```

## Build de Asset Bundles

Para utilizar a API, é necessário que os bundles do projeto sejam compilados e incluídos na pasta StreamingAssets/bundles do projeto. 
Uma forma comum de fazer esse processo é utilizando um script de CustomPreBuild para o processo de build no Go, como o exemplo a seguir:
   
```csharp
public class CustomPreBuild
{
    public static void PreBuildProcess()
    {
        BuildAssetBundles();

        AssetDatabase.Refresh();

        EditorApplication.Exit(0);
    }

    [MenuItem("Tools/Asset Bundle Simplified/Build Asset bundles")]
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

O modo de simulação utiliza o AssetDatabase para simular o uso da API no editor, permitindo que mudanças de assets sejam 
testadas mais facilmente sem a necessidade de fazer builds de novos bundles.

Para ligar e desligar o modo de simulação, basta ir no menu Tools/Asset Bundle Simplified e ligar/desligar o toogle de "Simulation Mode"

##Debug Window

A janela de debug permite visualizar em tempo real os bundles que estão carregados durante a execução do jogo **no editor**.
Também é possível verificar a contagem de referências para cada bundle filho.
Para abrir a janela de debug basta clicar em **Tools/Asset Bundle Simplified/DebugWindow**
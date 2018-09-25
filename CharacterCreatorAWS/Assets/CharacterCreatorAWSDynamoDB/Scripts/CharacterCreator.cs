using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using Amazon;
using Amazon.CognitoIdentity;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;

public class CharacterCreator : MonoBehaviour
{
    public Text characterNameText;
    public Text characterStatsText;
    public Text resultText;

    public Button createOperation;
    public Button refreshOperation;

    public Button NextCharacterButton;
    public Button PrevCharacterButton;

    public Button nextHair;
    public Button prevHair;

    public Button prevFace;
    public Button nextFace;

    public Button prevShirt;
    public Button nextShirt;

    public Button prevBody;
    public Button nextBody;

    public Button prevPants;
    public Button nextPants;

    public Button prevShoes;
    public Button nextShoes;

    public Image selectedHairImage;
    public Image selectedFaceImage;
    public Image selectedBodyImage;
    public Image selectedShirtImage;
    public Image selectedPantsImage;
    public Image selectedShoesImage;

    public InputField nameInput;
    public InputField ageInput;
    public InputField strengthInput;
    public InputField dexterityInput;
    public InputField intelligenceInput;

    // Add AWS specific variables here.
    public string cognitoIdentityPoolString;

    private CognitoAWSCredentials credentials;
    private IAmazonDynamoDB _client;
    private DynamoDBContext _context;

    private List<CharacterEntity> characterEntities = new List<CharacterEntity>();
    private int currentCharacterIndex;

    private string selectedName = string.Empty;
    private string selectedBody = string.Empty;
    private string selectedFace = string.Empty;
    private string selectedShirt = string.Empty;
    private string selectedHair = string.Empty;
    private string selectedPants = string.Empty;
    private string selectedShoes = string.Empty;

    private int age;
    private int strength;
    private int dexterity;
    private int intelligence;

    private List<string> hairList = new List<string>() { "hair1", "hair2", "hair3", "hair4", "hair5", "hair6", "hair7", "hair8", "hair9", "hair10", "hair11", "hair12", "hair13", "hair14" };
    private List<string> faceList = new List<string>() { "face1", "face2", "face3", "face4" };
    private List<string> bodyList = new List<string>() { "body1", "body2", "body3" };
    private List<string> pantsList = new List<string>() { "pants1", "pants2", "pants3", "pants4", "pants5", "pants6" };
    private List<string> shirtsList = new List<string>() { "shirt1", "shirt2", "shirt3", "shirt4", "shirt5", "shirt6", "shirt7", "shirt8", "shirt9" };
    private List<string> shoesList = new List<string>() { "shoes1", "shoes2", "shoes3", "shoes4" };

    /// <summary>
    /// Used to store all images found the resources folder at start.
    /// </summary>
    private Sprite[] allSprites;

    private DynamoDBContext Context
    {
        get
        {
            if (_context == null)
                _context = new DynamoDBContext(_client);

            return _context;
        }
    }

    void Awake()
    {
        // Setup listeners for some buttons in the scene.
        createOperation.onClick.AddListener(CreateCharacterInTable);
        refreshOperation.onClick.AddListener(FetchAllCharactersFromAWS);

        NextCharacterButton.onClick.AddListener(CycleNextCharacter);
        PrevCharacterButton.onClick.AddListener(CyclePrevCharacter);


        nextHair.onClick.AddListener(() => CycleNextClothing(nextHair));
        prevHair.onClick.AddListener(() => CyclePrevClothing(prevHair));

        nextFace.onClick.AddListener(() => CycleNextClothing(nextFace));
        prevFace.onClick.AddListener(() => CyclePrevClothing(prevFace));

        nextShirt.onClick.AddListener(() => CycleNextClothing(nextShirt));
        prevShirt.onClick.AddListener(() => CyclePrevClothing(prevShirt));

        nextBody.onClick.AddListener(() => CycleNextClothing(nextBody));
        prevBody.onClick.AddListener(() => CyclePrevClothing(prevBody));

        nextPants.onClick.AddListener(() => CycleNextClothing(nextPants));
        prevPants.onClick.AddListener(() => CyclePrevClothing(prevPants));

        nextShoes.onClick.AddListener(() => CycleNextClothing(nextShoes));
        prevShoes.onClick.AddListener(() => CyclePrevClothing(prevShoes));

        UnityInitializer.AttachToGameObject(this.gameObject);
        AWSConfigs.HttpClient = AWSConfigs.HttpClientOption.UnityWebRequest;
    }

    private void CyclePrevClothing(Button button)
    {
        // Clear current loaded character name as we are editing now
        characterNameText.text = "Editing character...";

        // We're using the gameobject tag as the pointer to what type of item this button cycles.
        var itemName = button.tag;

        if (string.IsNullOrEmpty(itemName)) return;

        var currentIndex = 0;
        Sprite selectedItem;
        switch (itemName)
        {
            case "hair":
                // Get the current selected item's position in the list, select the last one if we are at index 0, otherwise select the previous.
                currentIndex = hairList.IndexOf(selectedHair);
                selectedHair = currentIndex > 0 ? hairList[currentIndex - 1] : hairList.Last();

                // Assign the correct image as a sprite based on selection
                selectedItem = allSprites.First(image => image.name == selectedHair);
                selectedHairImage.overrideSprite = selectedItem;

                break;

            case "face":
                // Get the current selected item's position in the list, select the last one if we are at index 0, otherwise select the previous.
                currentIndex = faceList.IndexOf(selectedFace);
                selectedFace = currentIndex > 0 ? faceList[currentIndex - 1] : faceList.Last();

                // Assign the correct image as a sprite based on selection
                selectedItem = allSprites.First(image => image.name == selectedFace);
                selectedFaceImage.overrideSprite = selectedItem;

                break;

            case "shirt":
                // Get the current selected item's position in the list, select the last one if we are at index 0, otherwise select the previous.
                currentIndex = shirtsList.IndexOf(selectedShirt);
                selectedShirt = currentIndex > 0 ? shirtsList[currentIndex - 1] : shirtsList.Last();

                // Assign the correct image as a sprite based on selection
                selectedItem = allSprites.First(image => image.name == selectedShirt);
                selectedShirtImage.overrideSprite = selectedItem;

                break;

            case "body":
                // Get the current selected item's position in the list, select the last one if we are at index 0, otherwise select the previous.
                currentIndex = bodyList.IndexOf(selectedBody);
                selectedBody = currentIndex > 0 ? bodyList[currentIndex - 1] : bodyList.Last();

                // Assign the correct image as a sprite based on selection
                selectedItem = allSprites.First(image => image.name == selectedBody);
                selectedBodyImage.overrideSprite = selectedItem;

                break;

            case "pants":
                // Get the current selected item's position in the list, select the last one if we are at index 0, otherwise select the previous.
                currentIndex = pantsList.IndexOf(selectedPants);
                selectedPants = currentIndex > 0 ? pantsList[currentIndex - 1] : pantsList.Last();

                // Assign the correct image as a sprite based on selection
                selectedItem = allSprites.First(image => image.name == selectedPants);
                selectedPantsImage.overrideSprite = selectedItem;

                break;

            case "shoes":
                // Get the current selected item's position in the list, select the last one if we are at index 0, otherwise select the previous.
                currentIndex = shoesList.IndexOf(selectedShoes);
                selectedShoes = currentIndex > 0 ? shoesList[currentIndex - 1] : shoesList.Last();

                // Assign the correct image as a sprite based on selection
                selectedItem = allSprites.First(image => image.name == selectedShoes);
                selectedShoesImage.overrideSprite = selectedItem;

                break;
        }
    }

    private void CycleNextClothing(Button button)
    {
        // Clear current loaded character name as we are editing now
        characterNameText.text = "Editing character...";

        // We're using the gameobject tag as the pointer to what type of item this button cycles.
        var itemName = button.tag;

        if (string.IsNullOrEmpty(itemName)) return;

        var currentIndex = 0;
        Sprite selectedItem;
        switch (itemName)
        {
            case "hair":
                // Get the current selected item's position in the list, select the first one if we are at the last index, otherwise select the next.
                currentIndex = hairList.IndexOf(selectedHair);
                selectedHair = currentIndex < hairList.Count - 1 ? hairList[currentIndex + 1] : hairList.First();

                // Assign the correct image as a sprite based on selection
                selectedItem = allSprites.First(image => image.name == selectedHair);
                selectedHairImage.overrideSprite = selectedItem;

                break;

            case "face":
                // Get the current selected item's position in the list, select the first one if we are at the last index, otherwise select the next.
                currentIndex = faceList.IndexOf(selectedFace);
                selectedFace = currentIndex < faceList.Count - 1 ? faceList[currentIndex + 1] : faceList.First();

                // Assign the correct image as a sprite based on selection
                selectedItem = allSprites.First(image => image.name == selectedFace);
                selectedFaceImage.overrideSprite = selectedItem;

                break;

            case "shirt":
                // Get the current selected item's position in the list, select the first one if we are at the last index, otherwise select the next.
                currentIndex = shirtsList.IndexOf(selectedShirt);
                selectedShirt = currentIndex < shirtsList.Count - 1 ? shirtsList[currentIndex + 1] : shirtsList.First();

                // Assign the correct image as a sprite based on selection
                selectedItem = allSprites.First(image => image.name == selectedShirt);
                selectedShirtImage.overrideSprite = selectedItem;

                break;

            case "body":
                // Get the current selected item's position in the list, select the first one if we are at the last index, otherwise select the next.
                currentIndex = bodyList.IndexOf(selectedBody);
                selectedBody = currentIndex < bodyList.Count - 1 ? bodyList[currentIndex + 1] : bodyList.First();

                // Assign the correct image as a sprite based on selection
                selectedItem = allSprites.First(image => image.name == selectedBody);
                selectedBodyImage.overrideSprite = selectedItem;

                break;

            case "pants":
                // Get the current selected item's position in the list, select the first one if we are at the last index, otherwise select the next.
                currentIndex = pantsList.IndexOf(selectedPants);
                selectedPants = currentIndex < pantsList.Count - 1 ? pantsList[currentIndex + 1] : pantsList.First();

                // Assign the correct image as a sprite based on selection
                selectedItem = allSprites.First(image => image.name == selectedPants);
                selectedPantsImage.overrideSprite = selectedItem;

                break;

            case "shoes":
                // Get the current selected item's position in the list, select the first one if we are at the last index, otherwise select the next.
                currentIndex = shoesList.IndexOf(selectedShoes);
                selectedShoes = currentIndex < shoesList.Count - 1 ? shoesList[currentIndex + 1] : shoesList.First();

                // Assign the correct image as a sprite based on selection
                selectedItem = allSprites.First(image => image.name == selectedShoes);
                selectedShoesImage.overrideSprite = selectedItem;

                break;
        }
    }

    // Use this for initialization
    void Start()
    {
        // Load all sprites in the Kenney.nl resource folder.
        allSprites = Resources.LoadAll<Sprite>("Kenney.nl");

        selectedHair = selectedHairImage.overrideSprite.name;
        selectedFace = selectedFaceImage.overrideSprite.name;
        selectedBody = selectedBodyImage.overrideSprite.name;
        selectedShirt = selectedShirtImage.overrideSprite.name;
        selectedPants = selectedPantsImage.overrideSprite.name;
        selectedShoes = selectedShoesImage.overrideSprite.name;



        credentials = new CognitoAWSCredentials(cognitoIdentityPoolString, Amazon.RegionEndpoint.USEast2);
        credentials.GetIdentityIdAsync(delegate (AmazonCognitoIdentityResult<string> result)
        {
            if (result.Exception != null)
            {
                Debug.LogError("excepetion hit: " + result.Exception.Message);
            }

            var ddbClient = new AmazonDynamoDBClient(credentials, Amazon.RegionEndpoint.USEast2);

            resultText.text += ("\n*** Retrieving table information ***\n");

            var request = new DescribeTableRequest
            {
                TableName = @"CharacterCreator"
            };



            /*var response = ddbClient.DescribeTable(request);

            TableDescription description = response.Table;

            resultText.text += ("Name: " + description.TableName + "\n");
            resultText.text += ("# of items: " + description.ItemCount + "\n");
            resultText.text += ("Provision Throughput (reads/sec): " + description.ProvisionedThroughput.ReadCapacityUnits + "\n");
            resultText.text += ("Provision Throughput (reads/sec): " + description.ProvisionedThroughput.WriteCapacityUnits + "\n");*/

            /*ddbClient.DescribeTableRequest(request, (ddbresult) =>
            {

            }, null);*/

            ddbClient.DescribeTableAsync(request, (ddbresult) =>
            {
                if (result.Exception != null)
                {
                    resultText.text += result.Exception.Message;
                    Debug.Log(result.Exception);
                    return;
                }

                var response = ddbresult.Response;

                TableDescription description = response.Table;
                resultText.text += ("Name: " + description.TableName + "\n");
                resultText.text += ("# of items: " + description.ItemCount + "\n");
                resultText.text += ("Provision Throughput (reads/sec): " + description.ProvisionedThroughput.ReadCapacityUnits + "\n");
                resultText.text += ("Provision Throughput (reads/sec): " + description.ProvisionedThroughput.WriteCapacityUnits + "\n");
            }, null);

            _client = ddbClient;

            FetchAllCharactersFromAWS();
        });
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void LoadCharacter(CharacterEntity characterEntity)
    {
        selectedBody = characterEntity.BodySpriteName;
        selectedFace = characterEntity.FaceSpriteName;
        selectedHair = characterEntity.HairSpriteName;
        selectedPants = characterEntity.PantsSpriteName;
        selectedShirt = characterEntity.ShirtsSpriteName;
        selectedShoes = characterEntity.ShoesSpriteName;

        selectedHairImage.overrideSprite = allSprites.First(sprite => sprite.name == characterEntity.HairSpriteName);
        selectedFaceImage.overrideSprite = allSprites.First(sprite => sprite.name == characterEntity.FaceSpriteName);
        selectedBodyImage.overrideSprite = allSprites.First(sprite => sprite.name == characterEntity.BodySpriteName);
        selectedShirtImage.overrideSprite = allSprites.First(sprite => sprite.name == characterEntity.ShirtsSpriteName);
        selectedPantsImage.overrideSprite = allSprites.First(sprite => sprite.name == characterEntity.PantsSpriteName);
        selectedShoesImage.overrideSprite = allSprites.First(sprite => sprite.name == characterEntity.ShoesSpriteName);

        characterNameText.text = characterEntity.Name;
        nameInput.text = characterEntity.Name;
        ageInput.text = characterEntity.Age.ToString();
        strengthInput.text = characterEntity.Strength.ToString();
        dexterityInput.text = characterEntity.Dexterity.ToString();
        intelligenceInput.text = characterEntity.Intelligence.ToString();
    }

    private void CycleNextCharacter()
    {
        if (characterEntities.Count <= 0) return;

        if (currentCharacterIndex < characterEntities.Count - 1)
        {
            currentCharacterIndex++;
            LoadCharacter(characterEntities[currentCharacterIndex]);
        }
        else
        {
            LoadCharacter(characterEntities.First());
            currentCharacterIndex = 0;
        }
    }

    private void CyclePrevCharacter()
    {

        if (characterEntities.Count <= 0) return;

        if (currentCharacterIndex > 0)
        {
            currentCharacterIndex--;
            LoadCharacter(characterEntities[currentCharacterIndex]);
        }
        else
        {
            LoadCharacter(characterEntities.Last());
            currentCharacterIndex = characterEntities.Count - 1;
        }

    }

    private void FetchAllCharactersFromAWS()
    {
        resultText.text = "\n***LoadTable***";
        Table.LoadTableAsync(_client, "CharacterCreator", (loadTableResult) =>
        {
            if (loadTableResult.Exception != null)
            {
                resultText.text += "\n failed to load characters table";
                Debug.Log(loadTableResult.Exception);
            }
            else
            {
                try
                {
                    var context = Context;

                    var search = context.ScanAsync<CharacterEntity>(new ScanCondition("Age", ScanOperator.GreaterThan, 0));
                    search.GetRemainingAsync(result =>
                    {
                        if (result.Exception == null)
                        {
                            characterEntities = result.Result;

                            if (characterEntities.Count > 0) LoadCharacter(characterEntities.First());
                        }
                        else
                        {
                            Debug.LogError("Failed to get async table scan results: " + result.Exception.Message);
                        }
                    }, null);
                }
                catch (AmazonDynamoDBException exception)
                {
                    Debug.Log(string.Concat("Exception fetching characters from table:{0}", exception.Message));
                    Debug.Log(string.Concat("Error code: {0}, error type: {1}", exception.ErrorCode, exception.ErrorType));
                }
            }
        });
    }

    private void CreateCharacterInTable()
    {
        var newCharacter = new CharacterEntity
        {
            CharacterID = Guid.NewGuid().ToString(),
            BodySpriteName = selectedBody,
            FaceSpriteName = selectedFace,
            ShirtsSpriteName = selectedShirt,
            HairSpriteName = selectedHair,
            PantsSpriteName = selectedPants,
            ShoesSpriteName = selectedShoes,
            Name = nameInput.text,
            Age = Int32.Parse(ageInput.text),
            Dexterity = Int32.Parse(dexterityInput.text),
            Intelligence = Int32.Parse(intelligenceInput.text),
            Strength = Int32.Parse(strengthInput.text)
        };

        Context.SaveAsync(newCharacter, (result) =>
        {
            if (result.Exception == null)
                resultText.text += @"character saved";
        });
    }
}

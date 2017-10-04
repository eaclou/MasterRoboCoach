using UnityEngine;
using System.Collections;

public class TheGraphKing { // Does it need to be MonoBehaviour?
                            // All Hail The Graph King!
                            // 1 King Instance Per Player....

    // What does the Graph King Preside Over?
    // Three Graph Types
    // Each with a number of different modes

    // Fitness graph Panel
    //     Textures and data that is passed to shaders/materials
    public Texture2D texFitnessBasic;
    public Texture2D texFitnessAgentsLastGen;
    //public Texture2D texFitnessComponents;

    //public Texture2D texHistoryAvgGenomes;
    public float maxAspectRatio = 2f;
    //public Texture2D texHistoryAgentGenomes;

    //public Texture2D texCurAgentBrainDiagramAgent;
    //public Texture2D texCurAgentBrainDiagramTick;
    //public float curAgentBrainDiagramNumNodes = 1.0f;
    //public float curAgentBrainDiagramNumBiases = 1.0f;
    //public float curAgentBrainDiagramNumWeights = 1.0f;

    //public Material matFitnessBasic;
    //     -Or create both textures AND materials/shaders here and the UI simply points the current panel Material to TheGraphKing's mats?
    //     The DataVisPanelUI Class will probably call on the Graph King to return a Tex through a function?
    //     
    //How to make sure panel is updated only when it's needed?
    // When will DataClass change and Require new Textures to be created and passed to material?
    //     Only on New Generation?
    //     Or Per Agent? Per Player? All 3?

    // Historical Population Data Graph Panel

    // Current Agent Data Graphs Panel

    public TheGraphKing() {
        texFitnessBasic = new Texture2D(1, 1);
        texFitnessBasic.wrapMode = TextureWrapMode.Clamp;
        texFitnessAgentsLastGen = new Texture2D(1, 1);
        texFitnessAgentsLastGen.wrapMode = TextureWrapMode.Clamp;
        //texFitnessComponents = new Texture2D(1, 1);
        //texFitnessComponents.wrapMode = TextureWrapMode.Clamp;

        /*texHistoryAvgGenomes = new Texture2D(1, 1);
        texHistoryAvgGenomes.wrapMode = TextureWrapMode.Clamp;
        texHistoryAvgGenomes.filterMode = FilterMode.Bilinear;
        texHistoryAgentGenomes = new Texture2D(1, 1);
        texHistoryAgentGenomes.wrapMode = TextureWrapMode.Clamp;
        texHistoryAgentGenomes.filterMode = FilterMode.Point;

        texCurAgentBrainDiagramAgent = new Texture2D(1, 1);
        texCurAgentBrainDiagramAgent.wrapMode = TextureWrapMode.Clamp;
        texCurAgentBrainDiagramAgent.filterMode = FilterMode.Point;
        texCurAgentBrainDiagramTick = new Texture2D(1, 1);
        texCurAgentBrainDiagramTick.wrapMode = TextureWrapMode.Clamp;
        texCurAgentBrainDiagramTick.filterMode = FilterMode.Point;
        */
        //Debug.Log ("TheGraphKing LIVES!!!");
    }

    public void BuildTexturesFitnessPerGen(FitnessManager fitnessManager) {
        //DataManager dataManager = player.dataManager;
        //BuildTexturesFitnessBasic(dataManager);
        //BuildTexturesFitnessAgentsLastGen(dataManager);
        //BuildTexturesFitnessComponents(dataManager);

        BuildTexturesFitnessBasic(fitnessManager);
    }

    public void BuildTexturesHistoryPerGen() {
        //DataManager dataManager = player.dataManager;
        //BuildTexturesHistoryAvgGenomes(dataManager);
        //BuildTexturesHistoryAgentGenomes(player);
    }

    /*public void BuildTexturesCurAgentPerTick(Player player, MiniGameManager miniGameManager, int agentIndex) {
        DataManager dataManager = player.dataManager;
        BuildTexturesCurAgentBrainDiagramTick(player, miniGameManager, agentIndex);
    }

    public void BuildTexturesCurAgentPerAgent(Player player, int agentIndex) {
        DataManager dataManager = player.dataManager;
        BuildTexturesCurAgentBrainDiagramAgent(player, agentIndex);
    }*/

    //#region fitness graph methods
    public void BuildTexturesFitnessBasic(FitnessManager fitnessManager) {
        int texWidth = fitnessManager.alltimeBaselineVersusAvgScoreRatiosList.Count;
        texFitnessBasic.Resize(texWidth, 1);
        for (int y = 0; y < 1; y++) { // not needed for this due to 1 pixel height			
            for (int x = 0; x < texWidth; x++) {
                float pixValueMinRatio = fitnessManager.alltimeBaselineVersusMinScoreRatiosList[x] / fitnessManager.alltimeMaxRatioValue;
                float pixValueAvgRatio = fitnessManager.alltimeBaselineVersusAvgScoreRatiosList[x] / fitnessManager.alltimeMaxRatioValue;
                float pixValueMaxRatio = fitnessManager.alltimeBaselineVersusMaxScoreRatiosList[x] / fitnessManager.alltimeMaxRatioValue;
                
                texFitnessBasic.SetPixel(x, y, new Color(pixValueMinRatio, pixValueMaxRatio, pixValueAvgRatio));
            }
        }
        texFitnessBasic.Apply();

        //string fitValues = "Raw: ";
        //for (int x = 0; x < texWidth; x++) {
        //    fitValues += dataManager.generationDataList[x].avgAgentScoreRaw.ToString() + ", ";
        //}        
        Debug.Log ("TheGraphKing BuildTexturesFitnessBasic Length: " + texFitnessBasic.width.ToString());
    }

    public void BuildTexturesFitnessAgentsLastGen() {
        /*int genNumber;
        if (dataManager.generationDataList.Count < 2) {
            genNumber = 0;
        }
        else {
            genNumber = dataManager.generationDataList.Count - 2;
        }
        int texWidth = dataManager.generationDataList[genNumber].agentDataArray.Length;  // Get number of agents in the previous generation
        texFitnessAgentsLastGen.Resize(texWidth, 1);
        for (int y = 0; y < 1; y++) { // not needed for this due to 1 pixel height			
            for (int x = 0; x < texWidth; x++) {
                float pixValueRaw = dataManager.generationDataList[genNumber].agentDataArray[x].rawValueAvg;
                float pixValueWeighted = dataManager.generationDataList[genNumber].agentDataArray[x].weightedValueAvg;
                texFitnessAgentsLastGen.SetPixel(x, y, new Color(pixValueRaw, pixValueWeighted, 0f));
            }
        }
        texFitnessAgentsLastGen.Apply();

        //string fitValues = "Raw: ";
        //for(int x = 0; x < texWidth; x++) { 
        //	fitValues += dataManager.generationDataList[x].avgAgentScoreRaw.ToString() + ", ";
        //}
        //Debug.Log ("TheGraphKing BuildTexturesFitnessBasic Length: " + texFitnessBasic.width.ToString() + ". " + fitValues);
        */
    }
    /*
    public void BuildTexturesFitnessComponents(DataManager dataManager) {
        int genNumber;
        if (dataManager.generationDataList.Count < 2) {
            genNumber = 0;
        }
        else {
            genNumber = dataManager.generationDataList.Count - 2;
        }
        //int texWidth = dataManager.generationDataList.Count; // RE-INTRODUCE LATER!
        int texWidth = 1;
        int texHeight = dataManager.generationDataList[genNumber].totalNumFitnessComponents;
        int curTrialIndex = 0;
        int curFitnessComponentIndex = 0;
        int numTrials = dataManager.generationDataList[genNumber].agentDataArray[0].trialDataArray.Length;
        //float stackScore = 0f;
        //for(int i = 0; i <dataManager.generationDataList.Count;
        texFitnessComponents.Resize(texWidth, texHeight);
        for (int x = 0; x < texWidth; x++) {

            for (int y = 0; y < texHeight; y++) {
                if (curFitnessComponentIndex >= dataManager.generationDataList[genNumber].agentDataArray[0].trialDataArray[curTrialIndex].fitnessComponentDataArray.Length) {
                    curTrialIndex++;
                    curFitnessComponentIndex = 0;
                }
                float avgComponentScore = 0f;
                for (int i = 0; i < dataManager.generationDataList[genNumber].agentDataArray.Length; i++) {
                    avgComponentScore += dataManager.generationDataList[genNumber].agentDataArray[i].trialDataArray[curTrialIndex].fitnessComponentDataArray[curFitnessComponentIndex].rawValueAvg;
                }
                avgComponentScore /= (float)dataManager.generationDataList[genNumber].agentDataArray.Length;
                //if(curTrialIndex >= numTrials
                //stackScore += dataManager.generationDataList[genNumber]
                float pixValueRaw = avgComponentScore;
                float pixValueTrialFraction = (float)(curTrialIndex + 1) / (float)numTrials;
                float pixValueComponentFraction = (float)(curFitnessComponentIndex + 1f) / (float)dataManager.generationDataList[genNumber].agentDataArray[0].trialDataArray[curTrialIndex].fitnessComponentDataArray.Length;
                //Debug.Log ("TheGraphKing pixValueComponentFraction: " + pixValueComponentFraction.ToString());
                texFitnessComponents.SetPixel(x, y, new Color(pixValueRaw, pixValueTrialFraction, pixValueComponentFraction));


                curFitnessComponentIndex++;
            }

        }
        texFitnessComponents.Apply();

        string fitValues = "Raw: ";
        for (int x = 0; x < texWidth; x++) {
            fitValues += dataManager.generationDataList[x].avgAgentScoreRaw.ToString() + ", ";
        }
        //Debug.Log ("TheGraphKing BuildTexturesFitnessBasic Length: " + texFitnessBasic.width.ToString() + ". " + fitValues);
    }
    #endregion

    #region History Graph Methods:
    public void BuildTexturesHistoryAvgGenomes(DataManager dataManager) {
        int texWidth;
        int numGens = dataManager.generationDataList.Count;
        int biasLength = dataManager.generationDataList[0].genAvgGenome.genomeBiases.Length;
        int weightLength = dataManager.generationDataList[0].genAvgGenome.genomeWeights.Length;
        int texHeight = biasLength + weightLength;

        float aspectRatio = (float)numGens / (float)texHeight;
        if (aspectRatio > maxAspectRatio) {
            texWidth = (int)texHeight * (int)maxAspectRatio;
        }
        else {
            texWidth = numGens;
        }
        //Debug.Log ("Texture Size - X: " + texWidth.ToString() + ", Y: " + texHeight.ToString());
        texHistoryAvgGenomes.Resize(texWidth, texHeight);
        for (int x = 0; x < texWidth; x++) {
            int genIndex = Mathf.RoundToInt(((float)x / (float)texWidth) * numGens);
            for (int b = 0; b < biasLength; b++) {
                float pixValueRaw = dataManager.generationDataList[genIndex].genAvgGenome.genomeBiases[b];
                pixValueRaw = (pixValueRaw + 5f) / 10f; // get in 0-1 range
                texHistoryAvgGenomes.SetPixel(x, b, new Color(pixValueRaw, 0f, 0f));
            }
            for (int w = 0; w < weightLength; w++) {
                float pixValueRaw = dataManager.generationDataList[genIndex].genAvgGenome.genomeWeights[w];
                pixValueRaw = (pixValueRaw + 5f) / 10f; // get in 0-1 range
                texHistoryAvgGenomes.SetPixel(x, w + biasLength, new Color(pixValueRaw, 0f, 0f));
            }
        }
        texHistoryAvgGenomes.Apply();
    }

    public void BuildTexturesHistoryAgentGenomes(Player player) {
        int texWidth = player.masterPopulation.masterAgentArray.Length;
        int biasLength = player.masterPopulation.masterAgentArray[0].genome.genomeBiases.Length;
        int weightLength = player.masterPopulation.masterAgentArray[0].genome.genomeWeights.Length;
        int texHeight = biasLength + weightLength;
        //Debug.Log ("Texture Size - X: " + texWidth.ToString() + ", Y: " + texHeight.ToString());
        texHistoryAgentGenomes.Resize(texWidth, texHeight);
        for (int x = 0; x < texWidth; x++) {
            for (int b = 0; b < biasLength; b++) {
                float pixValueRaw = player.masterPopulation.masterAgentArray[x].genome.genomeBiases[b];
                //Debug.Log ("pixValueRaw: " + pixValueRaw.ToString());
                pixValueRaw = (pixValueRaw + 5f) / 10f; // get in 0-1 range
                texHistoryAgentGenomes.SetPixel(x, b, new Color(pixValueRaw, 0f, 0f));
            }
            for (int w = 0; w < weightLength; w++) {
                float pixValueRaw = player.masterPopulation.masterAgentArray[x].genome.genomeWeights[w];
                pixValueRaw = (pixValueRaw + 5f) / 10f; // get in 0-1 range
                texHistoryAgentGenomes.SetPixel(x, w + biasLength, new Color(pixValueRaw, 0f, 0f));
            }
        }
        texHistoryAgentGenomes.Apply();

    }
    #endregion

    #region Current Agent Graph Methods:
    public void BuildTexturesCurAgentBrainDiagramTick(Player player, MiniGameManager miniGameManager, int agentIndex) {
        int[] layerSizes = player.masterPopulation.masterAgentArray[agentIndex].genome.layerSizes;
        int totalNodes = 0;
        for (int i = 0; i < layerSizes.Length; i++) { // for each layer:
            for (int j = 0; j < layerSizes[i]; j++) { // For each node in that layer:
                totalNodes++;
            }
        }
        int texWidth = totalNodes;
        int texHeight = 1;
        curAgentBrainDiagramNumNodes = totalNodes;
        //Debug.Log ("numNodes: " + curAgentBrainDiagramNumNodes.ToString());
        texCurAgentBrainDiagramTick.Resize(texWidth, texHeight);

        int linearIndex = 0;
        for (int l = 0; l < layerSizes.Length; l++) {  // Loop through each brain Layer:

            for (int x = 0; x < layerSizes[l]; x++) {
                float pixValueRaw;
                if (l == 0) {  // input layer:
                    pixValueRaw = (miniGameManager.brainInput[x][0] + 1f) * 0.5f; // normalize to 0-1 range
                }
                else if (l == (layerSizes.Length - 1)) {  // output layer
                    pixValueRaw = (miniGameManager.brainOutput[x][0] + 1f) * 0.5f; // normalize to 0-1 range
                }
                else {  // hidden layer
                        //pixValueRaw = (player.masterPopulation.masterAgentArray[agentIndex].brain.layerOutput[l][x] + 1f) * 0.5f; // normalize to 0-1 range
                    pixValueRaw = (player.masterPopulation.masterAgentArray[agentIndex].brain.layerOutput[l - 1][x] + 1f) * 0.5f; // normalize to 0-1 range
                }
                texCurAgentBrainDiagramTick.SetPixel(linearIndex, 0, new Color(pixValueRaw, ((float)l + 1f) / (float)layerSizes.Length, ((float)x + 1f) / (float)layerSizes[l]));
                // R: node value,  G: inverse layer index,  B: inverse node index
                linearIndex++;
            }
        }
        texCurAgentBrainDiagramTick.Apply();
    }

    public void BuildTexturesCurAgentBrainDiagramAgent(Player player, int agentIndex) {

        int biasLength = player.masterPopulation.masterAgentArray[0].genome.genomeBiases.Length;
        int weightLength = player.masterPopulation.masterAgentArray[0].genome.genomeWeights.Length;
        curAgentBrainDiagramNumBiases = biasLength;
        curAgentBrainDiagramNumWeights = weightLength;
        int texWidth = biasLength + weightLength;
        int texHeight = 1;
        //int texHeight = biasLength + weightLength;
        //Debug.Log ("Texture Size - X: " + texWidth.ToString() + ", Y: " + texHeight.ToString());
        texCurAgentBrainDiagramAgent.Resize(texWidth, 1);
        //for(int x = 0; x < texWidth; x++) { 
        int index = 0;
        for (int b = 0; b < biasLength; b++) {
            float pixValueRaw = player.masterPopulation.masterAgentArray[agentIndex].genome.genomeBiases[b];
            //Debug.Log ("pixValueRaw: " + pixValueRaw.ToString());
            pixValueRaw = (pixValueRaw + 5f) / 10f; // get in 0-1 range
            texCurAgentBrainDiagramAgent.SetPixel(index, 0, new Color(pixValueRaw, 0f, 0f));
            index++;
        }
        for (int w = 0; w < weightLength; w++) {
            float pixValueRaw = player.masterPopulation.masterAgentArray[agentIndex].genome.genomeWeights[w];
            pixValueRaw = (pixValueRaw + 5f) / 10f; // get in 0-1 range
            texCurAgentBrainDiagramAgent.SetPixel(index, 0, new Color(pixValueRaw, 0f, 0f));
            index++;
        }
        texCurAgentBrainDiagramAgent.Apply();

    }
    #endregion
    */
}

using UnityEngine;

public class BlockSelector : MonoBehaviour
{
    public GameObject[] blockPrefabs; // Lista de prefabs de bloques
    private GameObject currentBlock; 

    public void SelectBlock(int index)
    {
        currentBlock = blockPrefabs[index]; // Selecciona el bloque
    }

    public GameObject GetSelectedBlock()
    {
        return currentBlock;
    }
}
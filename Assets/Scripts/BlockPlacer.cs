using UnityEngine;

public class BlockPlacer : MonoBehaviour
{
    public BlockSelector blockSelector; // Referencia al selector de bloques
    public LayerMask gridLayer; // Capa del suelo donde se colocarán los bloques

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Clic izquierdo para colocar
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100f, gridLayer))
            {
                Vector3 position = hit.point;
                position = new Vector3(Mathf.Round(position.x), 0.5f, Mathf.Round(position.z)); // Ajustar a la cuadrícula
                
                if (blockSelector.GetSelectedBlock() != null)
                {
                    Instantiate(blockSelector.GetSelectedBlock(), position, Quaternion.identity);
                }
            }
        }
    }
}
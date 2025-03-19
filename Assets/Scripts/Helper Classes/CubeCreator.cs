using UnityEngine;

public class CubeCreator : MonoBehaviour
{
    [SerializeField] private GameObject baseCube;
    [SerializeField] private Material baseMaterial;

	public Color cubeColor;
    [HideInInspector] public int cubeSize;

	public void CreateNewCube()
    {
        GameObject newCube = Instantiate(baseCube, Vector3.zero, Quaternion.identity);
        Cube cube = newCube.GetComponent<Cube>();
		cube.InitializeParams(cubeSize, cubeColor);

		Material newMaterial = new Material(baseMaterial);
		newMaterial.SetColor("Color_BE5A53D8", cubeColor);

		Vector3 newCubeSize = new Vector3(cubeSize, cubeSize, 1);
		cube.center.transform.localPosition *= cubeSize;
		foreach (Face face in cube.faces) {
			face.quad.transform.localScale = newCubeSize;
			face.quad.transform.localPosition *= cubeSize;
			face.quad.sharedMaterial = newMaterial;
		}
	}
}

//using UnityEngine;
//using UnityEngine.UI;

//public class Crosshair : MonoBehaviour
//{
//    [Header("Crosshair Settings")]
//    public Color crosshairColor = Color.white;
//    public float size = 20f; // length of each X line
//    public Canvas canvas;     // Assign your Canvas (Screen Space - Overlay)
//    public Image crosshairImagePrefab; // Assign a small X-shaped sprite (UI Image)

//    private Image crosshairX1;
//    private Image crosshairX2;

//    void Start()
//    {
//        // Hide the default system cursor
//        Cursor.visible = false;
//        Cursor.lockState = CursorLockMode.None;

//        if (canvas == null)
//        {
//            Debug.LogError("Crosshair: Canvas not assigned!");
//            return;
//        }

//        if (crosshairImagePrefab == null)
//        {
//            Debug.LogError("Crosshair: Crosshair Image Prefab not assigned!");
//            return;
//        }

//        // Instantiate the two lines of the X
//        crosshairX1 = Instantiate(crosshairImagePrefab, canvas.transform);
//        crosshairX2 = Instantiate(crosshairImagePrefab, canvas.transform);

//        crosshairX1.color = crosshairColor;
//        crosshairX2.color = crosshairColor;

//        // Rotate second line 90 degrees to form the X
//        crosshairX2.rectTransform.rotation = Quaternion.Euler(0, 0, 90);
//    }

//    void Update()
//    {
//        Vector3 mousePos = Input.mousePosition;

//        // Keep the crosshair at the mouse position
//        crosshairX1.transform.position = mousePos;
//        crosshairX2.transform.position = mousePos;

//        // Set the size of each line
//        crosshairX1.rectTransform.sizeDelta = new Vector2(size, 2);
//        crosshairX2.rectTransform.sizeDelta = new Vector2(size, 2);
//    }
//}

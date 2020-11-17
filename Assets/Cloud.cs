using UnityEngine;

public class Cloud : MonoBehaviour {
    public float speed;
    
    private void Update() {
        transform.position +=  speed * Time.deltaTime * Vector3.right;
        if (this.GetX() > 13) this.SetX(-13);
    }
}

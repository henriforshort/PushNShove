using UnityEngine;

public class Cloud : MonoBehaviour {
    public float speed;
    
    private void Update() {
        transform.position +=  speed * Time.deltaTime * Vector3.right;
        if (this.GetX() > 23) this.SetX(-23);
    }
}

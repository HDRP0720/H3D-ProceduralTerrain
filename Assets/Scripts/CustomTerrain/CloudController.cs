using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudController : MonoBehaviour
{
  public Color color;
  public Color lineColor;
  public int numberOfParticles;
  public float minSpeed;
  public float maxSpeed;
  public float distance;

  ParticleSystem cloudSystem;
  Vector3 startPosition;
  float speed;

  private void Start() 
  {
    cloudSystem = this.GetComponent<ParticleSystem>();
    Spawn();
  }
  private void Update() 
  {
    this.transform.Translate(0, 0, speed);

    if(Vector3.Distance(this.transform.position, startPosition) > distance)
    {
      Spawn();
    }
  }

  private void Spawn()
  {
    float xPos = UnityEngine.Random.Range(-0.5f, 0.5f);
    float yPos = UnityEngine.Random.Range(-0.5f, 0.5f);
    float zPos = UnityEngine.Random.Range(-0.5f, 0.5f);

    this.transform.localPosition = new Vector3(xPos, yPos, zPos);

    speed = UnityEngine.Random.Range(minSpeed, maxSpeed);
    startPosition = this.transform.position;
  }
}
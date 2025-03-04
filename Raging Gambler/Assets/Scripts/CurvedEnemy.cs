using UnityEngine;

public class CurvedEnemy : Enemy
{
    // Controls the intensity of the curve.
    public float curveAmplitude = 0.5f;
    // Controls how fast the curve oscillates.
    public float curveFrequency = 2f;

    protected override void FixedUpdate() // Overides from Enemy Controller
    {
        if (player != null)
        {
            // Get the straight-line direction to the player.
            Vector2 direction = ((Vector2)player.position - rb.position).normalized;

            // Calculate a perpendicular vector to the direction.
            Vector2 perpendicular = new Vector2(-direction.y, direction.x);

            // Calculate a curved offset using a sine wave.
            float offset = Mathf.Sin(Time.time * curveFrequency) * curveAmplitude;

            // Combine the forward direction with the perpendicular offset.
            Vector2 curvedDirection = (direction + perpendicular * offset).normalized;

            // Move using the curved direction.
            rb.MovePosition(rb.position + curvedDirection * speed * Time.fixedDeltaTime);
        }
    }
}

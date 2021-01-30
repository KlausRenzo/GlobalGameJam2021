using UnityEngine;

public class RoverInteraction : MonoBehaviour
{
	private Player player;
	private RoverEnergy roverEnergy;
	private Beacon beacon;

	private void Start()
	{
		player = this.GetComponent<Player>();
		roverEnergy = this.GetComponent<RoverEnergy>();

		player.InteractButtonPressed += PlayerOnInteractButtonPressed;
	}

	private void PlayerOnInteractButtonPressed()
	{
		if (beacon == null)
		{
			return;
		}

		if (roverEnergy.Energy > 1 && beacon.Activate())
		{
			roverEnergy.LosePiece();
		}
	}

	public void EnableBeaconActivation(Beacon beacon)
	{
		this.beacon = beacon;
	}

	public void DisableBeaconActivation(Beacon beacon)
	{
		beacon = null;
	}
}
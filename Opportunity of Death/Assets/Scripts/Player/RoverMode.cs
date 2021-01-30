using UnityEngine;

public class RoverMode : MonoBehaviour
{
	private Player player;
	private RoverEnergy roverEnergy;

	private void Start()
	{
		player = this.GetComponent<Player>();
		roverEnergy = GetComponent<RoverEnergy>();

		roverEnergy.EnergyLost += RoverEnergyOnEnergyLost;
	}

	private void RoverEnergyOnEnergyLost()
	{

	}
}
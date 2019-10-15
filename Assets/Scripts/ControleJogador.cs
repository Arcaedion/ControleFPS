using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class ControleJogador : MonoBehaviour
{
    [Header("Configuração de Camera")]
    [SerializeField]
    private Transform _posicaoCamera;

    [Header("Configuração de Controles")]
    [SerializeField]
    private float _sensibilidadeMouse = 5f;
    [SerializeField]
    private float _velocidadeJogadorPadrao = 5f;
    [SerializeField]
    private float _velocidadeJogadorCorrendo = 10f;
    [SerializeField]
    private float _forcaPulo = 5f;

    [Header("Configuração de Mundo")]
    [SerializeField]
    private float _gravidade = 10f;

    private float _velocidadeVertical = 0f;
    private CharacterController _controle;

    private float _anguloVertical;
    private float _anguloHorizontal;

    private bool _estaChao;
    private float _estaChaoTimer;
    void Start()
    {
        _controle = GetComponent<CharacterController>();
        _anguloVertical = 0f;
        _anguloHorizontal = transform.localEulerAngles.y;
    }

    // Update is called once per frame
    void Update()
    {
        var estaPulando = Input.GetButtonDown("Jump");
        var movimentoInput = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        var estaCorrendo = Input.GetButton("Run");

        AplicaPulo(estaPulando);
        AplicaMovimento(movimentoInput, estaCorrendo);
        AplicaCamera();
        AplicaGravidade();

    }

    private void AplicaPulo(bool estaPulando)
    {
        VerificaEstaNoChao();

        if(_estaChao && estaPulando)
        {
            _velocidadeVertical = _forcaPulo;
        }
    }

    private void VerificaEstaNoChao()
    {
        if(!_controle.isGrounded && _estaChao)
        {
            _estaChaoTimer += Time.deltaTime;
            if(_estaChaoTimer >= 0.5f)
            {
                _estaChao = false;
            }
        }
        else
        {
            _estaChaoTimer = 0f;
            _estaChao = true;
        }
    }

    private void AplicaCamera()
    {
        AplicaCameraX();
        AplicaCameraY();
    }

    private void AplicaCameraY()
    {
        var giraCamera = -Input.GetAxis("Mouse Y");
        giraCamera *= _sensibilidadeMouse;
        _anguloVertical = Mathf.Clamp(giraCamera + _anguloVertical, -89f, 89f);
        var angulosAtuais = _posicaoCamera.transform.localEulerAngles;
        angulosAtuais.x = _anguloVertical;
        _posicaoCamera.transform.localEulerAngles = angulosAtuais;
    }

    private void AplicaCameraX()
    {
        var viraJogador = Input.GetAxis("Mouse X") * _sensibilidadeMouse;
        _anguloHorizontal += viraJogador;

        if(_anguloHorizontal > 360)
            _anguloHorizontal -= 360f;
        if(_anguloHorizontal < 0)
        _anguloHorizontal += 360f;

        var angulosAtuais = transform.localEulerAngles;
        angulosAtuais.y = _anguloHorizontal;
        transform.localEulerAngles = angulosAtuais;

    }

    private void AplicaMovimento(Vector3 movimentoInput, bool correndo)
    {
        float velocidadeReal;
        if(correndo)
            velocidadeReal = _velocidadeJogadorCorrendo;
        else
            velocidadeReal = _velocidadeJogadorPadrao;

        if(movimentoInput.sqrMagnitude > 1f)
            movimentoInput.Normalize();

        var movimento = movimentoInput * velocidadeReal * Time.deltaTime;
        movimento = transform.TransformDirection(movimento);
        _controle.Move(movimento);
    }

    private void AplicaGravidade()
    {
        _velocidadeVertical = _velocidadeVertical - _gravidade * Time.deltaTime;
        if(_velocidadeVertical < -_gravidade)
        {
            _velocidadeVertical = -_gravidade; // velocidade maxima de queda
        }
        var velVertical = new Vector3(0, _velocidadeVertical * Time.deltaTime, 0);
        var flagColisao = _controle.Move(velVertical);
        if((flagColisao & CollisionFlags.Below) != 0)
        {
            _velocidadeVertical = 0;
        }
    }
}

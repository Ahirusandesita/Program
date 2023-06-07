using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//���܂����ł�����Ƀ��Z�b�g���鏈���͂Ȃ�

/// <summary>
/// ��ɃG���`�����g�o�^���邱�Ƃ��ł���
/// </summary>
interface IArrowEventSetting
{
    /// <summary>
    /// ��ɃG���`�����g�o�^����
    /// </summary>
    /// <param name="arrow">Arrow�N���X</param>
    /// <param name="needMoveChenge">Move�������X�V���邩</param>
    /// <param name="enchantmentState">�G���`�����g��Enum</param>
    void EventSetting(Arrow arrow, bool needMoveChenge, EnchantmentEnum.EnchantmentState enchantmentState);
}


/// <summary>
/// ��̃G���`�����g�̑g�ݍ��킹�����N���X
/// </summary>
public class ArrowEnchantment : MonoBehaviour, IArrowEventSetting
{
    #region �ϐ��錾��
    /// <summary>
    /// ��̌��ʃN���X
    /// </summary>
    public ArrowEnchant arrowEnchant;

    /// <summary>
    /// ��̃G�t�F�N�g�N���X
    /// </summary>
    public ArrowEnchantEffect arrowEnchantEffect;


    /// <summary>
    /// ��̈ړ��N���X
    /// </summary>
    private ArrowMove arrowMove;

    /// <summary>
    /// EnchantmentState�^�̗��z�z���������z��
    /// </summary>
    private EnchantmentEnum.EnchantmentState[] _enchantmentStateModels =
        new EnchantmentEnum.EnchantmentState[new EnchantmentEnum().EnchantmentStateLength()];

    /// <summary>
    /// �G���`�����g�𗝑z�z��ʂ�ɑ������z��
    /// </summary>
    private EnchantmentEnum.EnchantmentState[] _enchantmentStates =
        new EnchantmentEnum.EnchantmentState[new EnchantmentEnum().EnchantmentStateLength()];

    /// <summary>
    /// �G���`�����g���Z�b�g�����Ƃ��낪true�ɂȂ�z��
    /// </summary>
    private bool[] _isEnchantments = new bool[new EnchantmentEnum().EnchantmentStateLength()];

    /// <summary>
    /// �G���`�����g�̑g�ݍ��킹�݌v�}
    /// </summary>
    private EnchantmentEnum.EnchantmentState[,] _enchantPreparationNumbers =
    {
        {EnchantmentEnum.EnchantmentState.nomal  ,EnchantmentEnum.EnchantmentState.bomb   ,EnchantmentEnum.EnchantmentState.thunder    ,EnchantmentEnum.EnchantmentState.knockBack       ,EnchantmentEnum.EnchantmentState.homing         ,EnchantmentEnum.EnchantmentState.penetrate          },
        {EnchantmentEnum.EnchantmentState.nothing,EnchantmentEnum.EnchantmentState.nothing,EnchantmentEnum.EnchantmentState.bombThunder,EnchantmentEnum.EnchantmentState.bombKnockBack   ,EnchantmentEnum.EnchantmentState.bombHoming     ,EnchantmentEnum.EnchantmentState.bombPenetrate      },
        {EnchantmentEnum.EnchantmentState.nothing,EnchantmentEnum.EnchantmentState.nothing,EnchantmentEnum.EnchantmentState.nothing    ,EnchantmentEnum.EnchantmentState.thunderKnockBack,EnchantmentEnum.EnchantmentState.thunderHoming  ,EnchantmentEnum.EnchantmentState.thunderPenetrate   },
        {EnchantmentEnum.EnchantmentState.nothing,EnchantmentEnum.EnchantmentState.nothing,EnchantmentEnum.EnchantmentState.nothing    ,EnchantmentEnum.EnchantmentState.nothing         ,EnchantmentEnum.EnchantmentState.knockBackHoming,EnchantmentEnum.EnchantmentState.knockBackpenetrate },
        {EnchantmentEnum.EnchantmentState.nothing,EnchantmentEnum.EnchantmentState.nothing,EnchantmentEnum.EnchantmentState.nothing    ,EnchantmentEnum.EnchantmentState.nothing         ,EnchantmentEnum.EnchantmentState.nothing        ,EnchantmentEnum.EnchantmentState.homingPenetrate    },
        {EnchantmentEnum.EnchantmentState.nothing,EnchantmentEnum.EnchantmentState.nothing,EnchantmentEnum.EnchantmentState.nothing    ,EnchantmentEnum.EnchantmentState.nothing         ,EnchantmentEnum.EnchantmentState.nothing        ,EnchantmentEnum.EnchantmentState.nothing            },
    };

    /// <summary>
    /// ���݂̃G���`�����g��������ϐ�
    /// </summary>
    private EnchantmentEnum.EnchantmentState _enchantmentStateNow = EnchantmentEnum.EnchantmentState.nomal;

    #endregion


    private void Awake()
    {
        //�G���`�����g��Enum�̗��z�z������
        for (int i = 0; i < _enchantmentStateModels.Length; i++)
        {
            //���z�z��쐬
            _enchantmentStateModels[i] = (EnchantmentEnum.EnchantmentState)i;
            //������false ���z�ʂ��Enum��������ꂽ��true�ɂ���
            _isEnchantments[i] = false;
        }
        //�G���`�����g�����Z�b�g����
        EnchantmentStateReset();

    }

    /// <summary>
    /// ��̃G���`�����g������������
    /// </summary>
    /// <param name="arrow">Arrow�N���X</param>
    /// <param name="needMoveChenge">Move�������X�V���邩</param>
    /// <param name="enchantmentState">�G���`�����g��Enum</param>
    public void EventSetting(Arrow arrow, bool needMoveChenge, EnchantmentEnum.EnchantmentState enchantmentState)
    {
        //��I�u�W�F�N�g����Move���Q�b�g����
        arrowMove = arrow.gameObject.GetComponent<ArrowMove>();

        //Arrow�N���X�̃f���Q�[�g�ɑ�����鏈����Action�f���Q�[�g
        Action<Arrow.ArrowEnchantmentDelegateMethod, Arrow.ArrowEffectDelegateMethod, Arrow.MoveDelegateMethod> ArrowEnchant =
        (arrowEnchantMethod, arrowEffectMethod, arrowMoveMethod) =>
        {
            arrow._EventArrow = arrowEnchantMethod;
            arrow._EventArrowEffect = arrowEffectMethod;

            //�ړ����X�V���邩
            if (needMoveChenge)
            {
                arrow._MoveArrow = arrowMoveMethod;
            }
        };

        //�擾����Enchant��Enum���|�����킹�đ��
        EnchantmentEnum.EnchantmentState enchantState = EnchantmentStateSetting(enchantmentState);

        //���Enum���Z�b�g
        arrow.SetEnchantState(enchantState);

        //Enum�ɍ��킹�ď����������Ă���
        switch (enchantState)
        {
            case EnchantmentEnum.EnchantmentState.nomal:
                //�f���Q�[�g����p�f���Q�[�g�ϐ�
                ArrowEnchant(
                    //�G���`�����g�����֐����
                    new Arrow.ArrowEnchantmentDelegateMethod(arrowEnchant.ArrowEnchantment_Nomal),
                    //�G���`�����g�G�t�F�N�g�֐����
                    new Arrow.ArrowEffectDelegateMethod(arrowEnchantEffect.ArrowEffect_Nomal),
                    //�ړ��֐����
                    new Arrow.MoveDelegateMethod(arrowMove.ArrowMove_Nomal));
                break;

            //�ȉ�����Ă��邱�Ƃ͓����@�Ή������֐��������Ă���
            case EnchantmentEnum.EnchantmentState.bomb:
                ArrowEnchant(
                    new Arrow.ArrowEnchantmentDelegateMethod(arrowEnchant.ArrowEnchantment_Bomb),
                    new Arrow.ArrowEffectDelegateMethod(arrowEnchantEffect.ArrowEffect_Bomb),
                    new Arrow.MoveDelegateMethod(arrowMove.ArrowMove_Bomb));
                break;

            case EnchantmentEnum.EnchantmentState.thunder:
                ArrowEnchant(
                    new Arrow.ArrowEnchantmentDelegateMethod(arrowEnchant.ArrowEnchantment_Thunder),
                    new Arrow.ArrowEffectDelegateMethod(arrowEnchantEffect.ArrowEffect_Thunder),
                    new Arrow.MoveDelegateMethod(arrowMove.ArrowMove_Thunder));
                break;

            case EnchantmentEnum.EnchantmentState.knockBack:
                ArrowEnchant(
                    new Arrow.ArrowEnchantmentDelegateMethod(arrowEnchant.ArrowEnchantment_KnockBack),
                    new Arrow.ArrowEffectDelegateMethod(arrowEnchantEffect.ArrowEffect_KnockBack),
                    new Arrow.MoveDelegateMethod(arrowMove.ArrowMove_KnockBack));
                break;

            case EnchantmentEnum.EnchantmentState.homing:
                ArrowEnchant(
                    new Arrow.ArrowEnchantmentDelegateMethod(arrowEnchant.ArrowEnchantment_Homing),
                    new Arrow.ArrowEffectDelegateMethod(arrowEnchantEffect.ArrowEffect_Homing),
                    new Arrow.MoveDelegateMethod(arrowMove.ArrowMove_Homing));
                break;

            case EnchantmentEnum.EnchantmentState.penetrate:
                ArrowEnchant(
                    new Arrow.ArrowEnchantmentDelegateMethod(arrowEnchant.ArrowEnchantment_Penetrate),
                    new Arrow.ArrowEffectDelegateMethod(arrowEnchantEffect.ArrowEffect_Penetrate),
                    new Arrow.MoveDelegateMethod(arrowMove.ArrowMove_Penetrate));
                break;

            case EnchantmentEnum.EnchantmentState.bombThunder:
                ArrowEnchant(
                    new Arrow.ArrowEnchantmentDelegateMethod(arrowEnchant.ArrowEnchantment_BombThunder),
                    new Arrow.ArrowEffectDelegateMethod(arrowEnchantEffect.ArrowEffect_BombThunder),
                    new Arrow.MoveDelegateMethod(arrowMove.ArrowMove_BombThunder));
                break;

            case EnchantmentEnum.EnchantmentState.bombKnockBack:
                ArrowEnchant(
                    new Arrow.ArrowEnchantmentDelegateMethod(arrowEnchant.ArrowEnchantment_BombKnockBack),
                    new Arrow.ArrowEffectDelegateMethod(arrowEnchantEffect.ArrowEffect_BombKnockBack),
                    new Arrow.MoveDelegateMethod(arrowMove.ArrowMove_BombKnockBack));
                break;

            case EnchantmentEnum.EnchantmentState.bombHoming:
                ArrowEnchant(
                    new Arrow.ArrowEnchantmentDelegateMethod(arrowEnchant.ArrowEnchantment_BombHoming),
                    new Arrow.ArrowEffectDelegateMethod(arrowEnchantEffect.ArrowEffect_BombHoming),
                    new Arrow.MoveDelegateMethod(arrowMove.ArrowMove_BombHoming));
                break;

            case EnchantmentEnum.EnchantmentState.bombPenetrate:
                ArrowEnchant(
                    new Arrow.ArrowEnchantmentDelegateMethod(arrowEnchant.ArrowEnchantment_BombPenetrate),
                    new Arrow.ArrowEffectDelegateMethod(arrowEnchantEffect.ArrowEffect_BombPenetrate),
                    new Arrow.MoveDelegateMethod(arrowMove.ArrowMove_BombPenetrate));
                break;

            case EnchantmentEnum.EnchantmentState.thunderKnockBack:
                ArrowEnchant(
                    new Arrow.ArrowEnchantmentDelegateMethod(arrowEnchant.ArrowEnchantment_ThunderKnockBack),
                    new Arrow.ArrowEffectDelegateMethod(arrowEnchantEffect.ArrowEffect_ThunderKnockBack),
                    new Arrow.MoveDelegateMethod(arrowMove.ArrowMove_ThunderKnockBack));
                break;

            case EnchantmentEnum.EnchantmentState.thunderHoming:
                ArrowEnchant(
                    new Arrow.ArrowEnchantmentDelegateMethod(arrowEnchant.ArrowEnchantment_ThunderHoming),
                    new Arrow.ArrowEffectDelegateMethod(arrowEnchantEffect.ArrowEffect_ThunderHoming),
                    new Arrow.MoveDelegateMethod(arrowMove.ArrowMove_ThunderHoming));
                break;

            case EnchantmentEnum.EnchantmentState.thunderPenetrate:
                ArrowEnchant(
                    new Arrow.ArrowEnchantmentDelegateMethod(arrowEnchant.ArrowEnchantment_ThunderPenetrate),
                    new Arrow.ArrowEffectDelegateMethod(arrowEnchantEffect.ArrowEffect_ThunderPenetrate),
                    new Arrow.MoveDelegateMethod(arrowMove.ArrowMove_ThunderPenetrate));
                break;

            case EnchantmentEnum.EnchantmentState.knockBackHoming:
                ArrowEnchant(
                    new Arrow.ArrowEnchantmentDelegateMethod(arrowEnchant.ArrowEnchantment_KnockBackHoming),
                    new Arrow.ArrowEffectDelegateMethod(arrowEnchantEffect.ArrowEffect_KnockBackHoming),
                    new Arrow.MoveDelegateMethod(arrowMove.ArrowMove_KnockBackHoming));
                break;

            case EnchantmentEnum.EnchantmentState.knockBackpenetrate:
                ArrowEnchant(
                    new Arrow.ArrowEnchantmentDelegateMethod(arrowEnchant.ArrowEnchantment_KnockBackPenetrate),
                    new Arrow.ArrowEffectDelegateMethod(arrowEnchantEffect.ArrowEffect_KnockBackPenetrate),
                    new Arrow.MoveDelegateMethod(arrowMove.ArrowMove_KnockBackPenetrate));
                break;

            case EnchantmentEnum.EnchantmentState.homingPenetrate:
                ArrowEnchant(
                    new Arrow.ArrowEnchantmentDelegateMethod(arrowEnchant.ArrowEnchantment_HomingPenetrate),
                    new Arrow.ArrowEffectDelegateMethod(arrowEnchantEffect.ArrowEffect_HomingPenetrate),
                    new Arrow.MoveDelegateMethod(arrowMove.ArrowMove_HomingPenetrate));
                break;

            default:
                Debug.LogError("���݂��Ȃ��|�����킹");
                break;

        }
    }

    /// <summary>
    /// �G���`�����g�̗��z�z��Ƃ̈�v
    /// </summary>
    /// <param name="enchantmentState"></param>
    /// <returns></returns>
    private EnchantmentEnum.EnchantmentState EnchantmentStateSetting(EnchantmentEnum.EnchantmentState enchantmentState)
    {
        //�G���`�����g�̗��z�z�����v������true��Ή�����Y�����̔z��ɑ��
        for (int i = 0; i < _enchantmentStateModels.Length; i++)
        {
            //���z�z��ƈ�v���Ă��邩����
            if (_enchantmentStateModels[i] == enchantmentState)
            {
                //��v���Ă����true
                _isEnchantments[i] = true;
            }
        }

        return EnchantmentPreparation();

    }
    /// <summary>
    /// �G���`�����g��Enum�̑g�ݍ��킹����Enum���
    /// </summary>
    /// <returns>�|�����킹����EnchantState</returns>
    private EnchantmentEnum.EnchantmentState EnchantmentPreparation()
    {
        for (int i = 0; i < _isEnchantments.Length - 1; i++)
        {
            for (int j = i + 1; j < _isEnchantments.Length; j++)
            {
                if (_isEnchantments[i] && _isEnchantments[j])
                {
                    //2��true�̂Ƃ���i,j�ɓo�^���Ă���Enum��Now�ɑ��
                    _enchantmentStateNow = _enchantPreparationNumbers[i, j];
                }
            }
        }
        return _enchantmentStateNow;
    }

    /// <summary>
    /// �G���`�����g��State�����Z�b�g����@�G���`�����g�Ώۂ̖�ύX����鎞�ɌĂ�
    /// </summary>
    public void EnchantmentStateReset()
    {
        for (int i = 0; i < _enchantmentStates.Length; i++)
        {
            _isEnchantments[i] = false;
        }
        _isEnchantments[0] = true;
    }

}

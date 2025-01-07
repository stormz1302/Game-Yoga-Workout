using System;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;

namespace CompleteProject
{
    public class Purchaser : MonoBehaviour, IDetailedStoreListener
    {
        public static Purchaser instance;
        public static IStoreController m_StoreController;
        private static IExtensionProvider m_StoreExtensionProvider;
        public string removeAds;

        void Awake()
        {
            if (instance == null)
                instance = this;

            removeAds = Application.identifier + ".removeads";

            if (m_StoreController == null)
            {
                InitializePurchasing();
            }
        }

        public void InitializePurchasing()
        {
            if (IsInitialized())
                return;

            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
            builder.AddProduct(removeAds, ProductType.NonConsumable);
            UnityPurchasing.Initialize(this, builder); // Sử dụng IDetailedStoreListener
        }

        public static bool IsInitialized()
        {
            return m_StoreController != null && m_StoreExtensionProvider != null;
        }

        public void BuyFunc()
        {
            BuyProductID(DataParam.packBuyIAP);
            Debug.LogError("Purchase initiated: " + DataParam.packBuyIAP);
        }

        public void BuyProductID(string productId)
        {
            if (IsInitialized())
            {
                Product product = m_StoreController.products.WithID(productId);

                if (product != null && product.availableToPurchase)
                {
                    Debug.Log($"Purchasing product asynchronously: {product.definition.id}");
                    m_StoreController.InitiatePurchase(product);
                }
                else
                {
                    Debug.Log("BuyProductID: FAIL. Product not found or unavailable.");
                }
            }
            else
            {
                Debug.Log("BuyProductID FAIL. Not initialized.");
            }
        }

        public void RestorePurchases()
        {
            if (!IsInitialized())
            {
                Debug.Log("RestorePurchases FAIL. Not initialized.");
                return;
            }

            if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.OSXPlayer)
            {
                Debug.Log("RestorePurchases started...");

                var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();

                apple.RestoreTransactions((result, message) =>
                {
                    if (result)
                    {
                        Debug.Log("RestorePurchases successful.");
                    }
                    else
                    {
                        Debug.LogError($"RestorePurchases failed. Message: {message}");
                    }
                });
            }
            else
            {
                Debug.Log($"RestorePurchases FAIL. Not supported on platform: {Application.platform}");
            }
        }


        public void BtnRestore()
        {
            if (!IsInitialized())
            {
                Debug.Log("RestorePurchases FAIL. Not initialized.");
                return;
            }

            Product product = m_StoreController.products.WithID(removeAds);
            if (product != null && product.hasReceipt)
            {
                DataManager.instance.RemoveAdsFunc();
            }
        }

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            m_StoreController = controller;
            m_StoreExtensionProvider = extensions;
            Debug.Log("Unity IAP initialized successfully.");
        }

        // Triển khai cả hai phiên bản của OnInitializeFailed
        public void OnInitializeFailed(InitializationFailureReason error)
        {
            Debug.LogError($"Initialization failed: {error}");
        }

        public void OnInitializeFailed(InitializationFailureReason error, string message)
        {
            Debug.LogError($"Initialization failed: {error}, Message: {message}");
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
        {
            Debug.Log($"ProcessPurchase: PASS. Product: {args.purchasedProduct.definition.id}");

            if (String.Equals(args.purchasedProduct.definition.id, removeAds, StringComparison.Ordinal))
            {
                DataManager.instance.RemoveAdsFunc();
            }
            return PurchaseProcessingResult.Complete;
        }

        // Triển khai phương thức OnPurchaseFailed yêu cầu bởi IDetailedStoreListener
        public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
        {
            Debug.LogError($"OnPurchaseFailed: FAIL. Product: {product.definition.storeSpecificId}, Reason: {failureDescription.reason}, Message: {failureDescription.message}");
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            throw new NotImplementedException();
        }
    }
}

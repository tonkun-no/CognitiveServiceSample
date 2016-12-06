using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;

namespace FaceLogin.Core
{
    public class CognitiveServiceHelper
    {
        // FaceAPIクライアント
        FaceServiceClient _faceServiceClient = new FaceServiceClient("your subscription keys");
        // 予め用意したFaceListのID
        string _faceListId = "your facelistid";
        // 検出件数
        const int candidatesCount = 1;

        /// <summary>
        /// 顔認証を行います
        /// </summary>
        /// <param name="imageStream"></param>
        /// <returns></returns>
        public async Task<bool> FaceLogin(Stream imageStream)
        {
            var similarFaces = await GetSimilarFaces(imageStream);
            if (similarFaces.Length > 0)
            {
                if (similarFaces.FirstOrDefault().Confidence >= 0.5d)
                    // 検出できた場合は認証成功
                    return true;
            }
            // 以外は全て認証失敗
            return false;
        }

        /// <summary>
        /// FaceAPIを使用して似た顔を取得します
        /// </summary>
        /// <param name="imageStream"></param>
        /// <returns></returns>
        private async Task<SimilarPersistedFace[]> GetSimilarFaces(Stream imageStream)
        {
            try
            {
                // 顔を検出
                var faces = await _faceServiceClient.DetectAsync(imageStream);

                // 検出した顔がリストにあるか検索(1件のみ抽出)
                return await _faceServiceClient.FindSimilarAsync(faces.FirstOrDefault().FaceId, _faceListId, candidatesCount);
            }
            catch (FaceAPIException)
            {
                // 何かしらエラー
                return new SimilarPersistedFace[0];
            }
        }
    }
}

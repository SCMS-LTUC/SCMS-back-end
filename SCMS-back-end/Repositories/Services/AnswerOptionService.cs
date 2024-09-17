﻿
using SCMS_back_end.Models;
using Microsoft.EntityFrameworkCore;
using SCMS_back_end.Data;
using SCMS_back_end.Repositories.Interfaces;

namespace SCMS_back_end.Services
{
    public class AnswerOptionService : IAnswerOptionRepository
    {
        private readonly StudyCenterDbContext _context;

        public AnswerOptionService(StudyCenterDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AnswerOption>> GetAnswerOptionsByQuestionIdAsync(int questionId)
        {
            return await _context.AnswerOptions
                                 .Where(ao => ao.QuestionId == questionId)
                                 .ToListAsync();
        }

        public async Task<AnswerOption> GetAnswerOptionByIdAsync(int answerOptionId)
        {
            return await _context.AnswerOptions
                                 .FirstOrDefaultAsync(ao => ao.AnswerOptionId == answerOptionId);
        }

        public async Task AddAnswerOptionAsync(AnswerOption answerOption)
        {
            _context.AnswerOptions.Add(answerOption);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAnswerOptionAsync(AnswerOption answerOption)
        {
            _context.AnswerOptions.Update(answerOption);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAnswerOptionAsync(int answerOptionId)
        {
            var answerOption = await GetAnswerOptionByIdAsync(answerOptionId);
            if (answerOption != null)
            {
                _context.AnswerOptions.Remove(answerOption);
                await _context.SaveChangesAsync();
            }
        }
    }
}
